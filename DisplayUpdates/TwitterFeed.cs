using System;
using TweetSharp;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Concurrency;

namespace DisplayUpdates
{
    public interface ITwitterFeed
    {
        IObservable<TwitterStatus> Tweets { get; }
    }
    public class TwitterFeed : ITwitterFeed 
    {
        public TwitterFeed(Tuple<string,string> authKeys)
        {
            IScheduler sched = Scheduler.TaskPool;

            TwitterService service = new TwitterService(authKeys.Item1,authKeys.Item2);
            OAuthRequestToken requestToken = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestToken);
            var taw = new TwitterAuth() { AuthUrl = uri };
            taw.ShowDialog();
            OAuthAccessToken access = service.GetAccessToken(requestToken, taw.Token);
            service.AuthenticateWith(access.Token, access.TokenSecret);


            IEnumerable<TwitterStatus> tweets = service.ListTweetsOnHomeTimeline();
            var sinceId = tweets.Last().Id;
            var futureTweets = Observable.Create<TwitterStatus>(obs =>
            {
                bool isRunning = true;

                Action<Action<TimeSpan>> RecSelf = (self) =>
                {
                    if (isRunning)
                    {
                        var newtweets = service.ListTweetsOnHomeTimelineSince(sinceId);
                        if (newtweets.Any())
                        {
                            sinceId = newtweets.Last().Id;
                            foreach (var tweet in newtweets)
                            {
                                obs.OnNext(tweet);
                            }
                        }

                        TimeSpan sleepTime = GetSleepTime(service,sched);
                        if (isRunning)
                        {
                            self(sleepTime);
                        }
                    }
                };
                sched.Schedule(RecSelf, GetSleepTime(service, sched));

                return () => { isRunning = false; };
            });

            Tweets = tweets.ToObservable().Concat(futureTweets)
                    .ReplayLastByKey(tws => tws.User);
        }

        private TimeSpan GetSleepTime(TwitterService service, IScheduler sched)
        {
            TwitterRateLimitStatus rls = service.Response.RateLimitStatus;
            TimeSpan timeTillReset = rls.ResetTime - sched.Now;
            var secondsOfSleepTime = timeTillReset.TotalSeconds / (rls.RemainingHits == 0 ? 1 : rls.RemainingHits);
            return TimeSpan.FromSeconds(secondsOfSleepTime);
        }
        

        public IObservable<TwitterStatus> Tweets { get; private set; }
    }
}
