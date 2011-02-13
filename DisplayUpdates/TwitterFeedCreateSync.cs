using System;
using System.Concurrency;
using System.Linq;
using TweetSharp;
using System.Collections.Generic;

namespace DisplayUpdates
{
    public class TwitterFeedCreateSync : TwitterFeedBase
    {
        public TwitterFeedCreateSync(Tuple<string, string> authKeys):base(authKeys)
        {
             IEnumerable<TwitterStatus> tweets = service.ListTweetsOnHomeTimeline();
             long sinceId = GetMaxId(tweets, 0);
            
            var futureTweets = Observable.Create<TwitterStatus>( obs =>
                {
                    bool isRunning = true;
                    Action<Action<TimeSpan>> RecSelf = (self) =>
                    {
                        if (!isRunning)
                            return;

                        var newtweets = service.ListTweetsOnHomeTimelineSince(sinceId);
                        sinceId = GetMaxId(newtweets,sinceId);
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
    }
}
