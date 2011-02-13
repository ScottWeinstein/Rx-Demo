using System;
using TweetSharp;

namespace DisplayUpdates
{
    public interface ITwitterFeed
    {
        IObservable<TwitterStatus> Tweets { get; }
    }
}
