namespace DisplayUpdates
{
    using System;
    using System.Collections.ObjectModel;
    using TweetSharp;

    public class TwitterFeedVM
    {
        public TwitterFeedVM(IObservable<TwitterStatus> tweetxs)
        {
            Tweets = new ObservableCollection<TwitterStatus>();
            Tweets.MergeInsert(tweetxs, tws => tws.User.Id);
        }

        public ObservableCollection<TwitterStatus> Tweets { get; private set; }
    }
}
