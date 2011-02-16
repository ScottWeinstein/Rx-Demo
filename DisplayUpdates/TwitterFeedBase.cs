using System;
using System.Concurrency;
using TweetSharp;
using System.Collections.Generic;
using System.Linq;

namespace DisplayUpdates
{
    public abstract class TwitterFeedBase : ITwitterFeed
    {
        protected readonly TwitterService service;
        protected readonly IScheduler sched;

        public TwitterFeedBase(Tuple<string, string> authKeys)
        {
            sched = Scheduler.TaskPool;
            service = GetTwitterService(authKeys);
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

        protected static TimeSpan GetSleepTime(TwitterService service, IScheduler sched)
        {
            TwitterRateLimitStatus rls = service.Response.RateLimitStatus;
            return GetSleepTime(rls, sched);
        }

        protected static TimeSpan GetSleepTime(TwitterRateLimitStatus rls, IScheduler sched)
        {
            if (rls.HourlyLimit == -1)
                return TimeSpan.Zero;

            TimeSpan timeTillReset = rls.ResetTime - sched.Now;
            int remainingHits = rls.RemainingHits == 0 ? 1 : rls.RemainingHits;
            var secondsOfSleepTime = timeTillReset.TotalSeconds / remainingHits;
            return TimeSpan.FromSeconds(secondsOfSleepTime);
        }

        public IObservable<TwitterStatus> Tweets { get; protected set; }

        protected static long GetMaxId(IEnumerable<TwitterStatus> tweets, long sinceId)
        {
            return tweets.Max(tw => (long?)tw.Id) ?? sinceId;
        }
    }
}
