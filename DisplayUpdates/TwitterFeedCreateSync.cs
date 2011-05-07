namespace DisplayUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using TweetSharp;

    public class TwitterFeedCreateSync : TwitterFeedBase
    {
        public TwitterFeedCreateSync(Tuple<string, string> authKeys) : base(authKeys)
        {
             IEnumerable<TwitterStatus> tweets = 
                 service.ListTweetsOnHomeTimeline();
             long sinceId = GetMaxId(tweets, 0);

             var futureTweets =
             Observable.Create<TwitterStatus>(
             obs =>
             {
                 bool isRunning = true;
                 Action<Action<TimeSpan>> RecSelf = (self) =>
                 {
                     if (!isRunning)
                         return;

                     var newtweets = 
                         service.ListTweetsOnHomeTimelineSince(sinceId);
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
                 sched.Schedule(GetSleepTime(service, sched), RecSelf);

                 return () => { isRunning = false; };
             });

             Tweets = tweets.ToObservable()
                            .Concat(futureTweets)
                            .ReplayLastByKey(tws => tws.User)
                            .Publish()
                            .RefCount();
        }
    }
}
