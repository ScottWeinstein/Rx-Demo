using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Net;
using TweetSharp;

namespace DisplayUpdates
{
    public class TwitterFeed : ITwitterFeed 
    {
        public TwitterFeed(Tuple<string, string> authKeys)
        {
            IScheduler sched = Scheduler.TaskPool;
            TwitterService service = GetTwitterService(authKeys);

            var futureTweets = Observable.Create<TwitterStatus>(obs =>
                {
                    bool isRunning = true;
                    long? sinceId = null;

                    Action<Action<TimeSpan>> RecSelf = (self) =>
                    {
                        if (!isRunning)
                            return;

                        Action<IEnumerable<TwitterStatus>, TwitterResponse> processNewTweets = 
                            (newtweets, response) => 
                            {
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    sinceId = newtweets.Select(ts => (long?)ts.Id).Max() ?? sinceId;
                                    foreach (var tweet in newtweets)
                                    {
                                        obs.OnNext(tweet);
                                    }
                                }
                                else
                                {
                                    obs.OnError(response.InnerException);
                                }

                                if (isRunning)
                                {
                                    self(GetSleepTime(response.RateLimitStatus,sched));
                                }
                            };

                        if (sinceId.HasValue)
                        {
                            service.ListTweetsOnHomeTimelineSince((long)sinceId, processNewTweets);
                        }
                        else
                        {
                            service.ListTweetsOnHomeTimeline(processNewTweets);
                        }
                    };

                    sched.Schedule(RecSelf, GetSleepTime(service, sched));
                    return () => { isRunning = false; };
                });
            
            Tweets = futureTweets.ReplayLastByKey(tws => tws.User);
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
            return GetSleepTime(rls, sched);
        }
        
        private static TimeSpan GetSleepTime(TwitterRateLimitStatus rls, IScheduler sched)
        {
            if (rls.HourlyLimit == -1)
                return TimeSpan.Zero;

            TimeSpan timeTillReset = rls.ResetTime - sched.Now;
            var secondsOfSleepTime = timeTillReset.TotalSeconds / (rls.RemainingHits == 0 ? 1 : rls.RemainingHits);
            return TimeSpan.FromSeconds(secondsOfSleepTime);
        }

        public IObservable<TwitterStatus> Tweets { get; private set; }
    }
}
/* With Create
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
*/


/* Generate With Time
IObservable<TwitterStatus> futureTweets = Observable.GenerateWithTime(state,
                                                                    _ => true,
                                                                    st =>
                                                                    {
                                                                        var sinceId = st.Item2;
                                                                        var newtweets = service.ListTweetsOnHomeTimelineSince(sinceId);
                                                                        sinceId = GetMaxId(newtweets, sinceId);
                                                                        return new Tuple<TwitterService, long, IEnumerable<TwitterStatus>>(service, sinceId, newtweets);
                                                                    },
                                                                    st => st.Item3.ToObservable(),
                                                                    st => GetSleepTime(st.Item1, sched), 
                                                                    sched)
                                                                .SelectMany(a => a);

            */