using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Net;
using TweetSharp;

namespace DisplayUpdates
{
    public class TwitterFeedAsync : TwitterFeedBase
    {
        public TwitterFeedAsync(Tuple<string, string> authKeys):base(authKeys)
        {
            var futureTweets =
            Observable.Create<TwitterStatus>(
            obs =>
            {
                bool isRunning = true;
                long? sinceId = null;

                Action<Action<TimeSpan>> 
                RecSelf = (self) =>
                {
                    if (!isRunning) return;

                    Action<IEnumerable<TwitterStatus>, TwitterResponse>
                        processNewTweets = (newtweets, response) =>
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
                                self(GetSleepTime(response.RateLimitStatus, sched));
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

            Tweets = futureTweets
                        .ReplayLastByKey(tws => tws.User)
                        .Publish()
                        .RefCount();
        }
    }
}
