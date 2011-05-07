namespace DisplayUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using TweetSharp;

    public class TwitterFeedGenerateSync : TwitterFeedBase
    {
        public TwitterFeedGenerateSync(Tuple<string, string> authKeys)
            : base(authKeys)
        {
            IEnumerable<TwitterStatus> tweets = 
                service.ListTweetsOnHomeTimeline();

            var state = Tuple.Create(service, GetMaxId(tweets, 0), tweets);

            Func<Tuple<TwitterService, long, IEnumerable<TwitterStatus>>,TimeSpan> newVariable = (st) => GetSleepTime(st.Item1, sched);

            IObservable<TwitterStatus> futureTweets =
                Observable.Generate(
                 state,         //initialState
                 _ => true,     //condition
                 st =>          //iterate
                 {
                     var sinceId = st.Item2;
                     var newtweets =
                         service.ListTweetsOnHomeTimelineSince(sinceId);
                     sinceId = GetMaxId(newtweets, sinceId);
                     return Tuple.Create(service, sinceId, newtweets);
                 },
                st => st.Item3.ToObservable(),          //resultSelector
                newVariable,    //timeSelector
                sched)                                  //scheduler
                .SelectMany(a => a); // need to flatten IO<IO<T> to just IO<T>

            Tweets = tweets.ToObservable()
                           .Concat(futureTweets)
                           .ReplayLastByKey(tws => tws.User)
                           .Publish()
                           .RefCount();
        }
    }
}
