namespace DisplayUpdates
{
    using System;
    using TweetSharp;

    public interface ITwitterFeed
    {
        IObservable<TwitterStatus> Tweets { get; }
    }
}
