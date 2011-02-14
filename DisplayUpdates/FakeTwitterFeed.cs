using System;
using TweetSharp;
using System.Linq;

namespace DisplayUpdates
{
    public class FakeTwitterFeed : ITwitterFeed
    {
        private readonly string[] _ScreenNames;
        public FakeTwitterFeed()
        {
            _ScreenNames = new[] { "@A", "@B", "@C", "@D", "@E", "@F", "@G", "@H" };
            Random rnd = new Random();
            Tweets = Observable.GenerateWithTime(rnd.Next(),
                _ => true,
                _ => rnd.Next(),
                MakeTwitterStatus,
                _ => TimeSpan.FromSeconds(0.1))
                .ReplayLastByKey(tws => tws.User);
        }

        private TwitterStatus MakeTwitterStatus(int ii)
        {
            int index = ii % _ScreenNames.Length;
            TwitterUser newUser = new TwitterUser { Id = index, ScreenName = _ScreenNames[index] };
            return new TwitterStatus { Id = ii, User = newUser, Text = " " + ii };
        }

        public IObservable<TwitterStatus> Tweets
        {
            get;
            private set;
        }
    }
}
