using System;
using TweetSharp;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Concurrency;

namespace DisplayUpdates
{
    public class TwitterFeed : ITwitterFeed 
    {
        public TwitterFeed(Tuple<string, string> authKeys)
        {
            IScheduler sched = Scheduler.TaskPool;
            TwitterService service = GetTwitterService(authKeys);

            IEnumerable<TwitterStatus> tweets = service.ListTweetsOnHomeTimeline();
            var sinceId = GetMaxId(tweets, 0);

            IObservable<TwitterStatus> futureTweets =
                Observable.Create<TwitterStatus>(
                obs =>
                {
                    bool isRunning = true;
                    Action<Action<TimeSpan>> RecSelf = (self) =>
                    {
                        if (!isRunning)
                            return;
                        
                        var newtweets = service.ListTweetsOnHomeTimelineSince(sinceId);
                        sinceId = GetMaxId(newtweets, sinceId);

                        foreach (var tweet in newtweets)
                        {
                            obs.OnNext(tweet);
                        }

                        if (isRunning)
                        {
                            self(GetSleepTime(service, sched));
                        }
                        
                    };
                    sched.Schedule(RecSelf, GetSleepTime(service, sched));

                    return () => { isRunning = false; };
                });

            Tweets = tweets.ToObservable().Concat(futureTweets)
                    .ReplayLastByKey(tws => tws.User);
        }

        private static long GetMaxId(IEnumerable<TwitterStatus> newtweets, long sinceId)
        {
            var newId = newtweets.Select(ts => (long?)ts.Id).Max() ?? sinceId;
            return newId;

        }

        private static TwitterService GetTwitterService(Tuple<string, string> authKeys)
        {
            TwitterService service = new TwitterService(authKeys.Item1, authKeys.Item2);
            OAuthRequestToken requestToken = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestToken);
            var taw = new TwitterAuth() { AuthUrl = uri };
            taw.ShowDialog();
            OAuthAccessToken access = service.GetAccessToken(requestToken, taw.Token);
            service.AuthenticateWith(access.Token, access.TokenSecret);
            return service;
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
