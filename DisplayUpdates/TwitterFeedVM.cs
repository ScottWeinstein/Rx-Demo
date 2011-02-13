using System;
using TweetSharp;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace DisplayUpdates
{
    public class TwitterFeedVM
    {
        public TwitterFeedVM(IObservable<TwitterStatus> tweetxs)
        {
            Tweets = new ObservableCollection<TwitterStatus>();
            Tweets.MergeInsert(tweetxs, tws => tws.User);
        }

        public ObservableCollection<TwitterStatus> Tweets { get; private set; }


    }
}
