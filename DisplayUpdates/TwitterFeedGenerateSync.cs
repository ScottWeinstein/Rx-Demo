using System;
using System.Collections.Generic;
using System.Linq;
using TweetSharp;

namespace DisplayUpdates
{
    public class TwitterFeedGenerateSync : TwitterFeedBase
    {
        public TwitterFeedGenerateSync(Tuple<string, string> authKeys)
            : base(authKeys)
        {
            IEnumerable<TwitterStatus> tweets = service.ListTweetsOnHomeTimeline();
            var state = new Tuple<TwitterService, long, IEnumerable<TwitterStatus>>(service, GetMaxId(tweets, 0), tweets);
            IObservable<TwitterStatus> futureTweets = 
                Observable.GenerateWithTime(state,
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

            Tweets = tweets.ToObservable()
                            .Concat(futureTweets)
                            .ReplayLastByKey(tws => tws.User);
        }
    }
}
