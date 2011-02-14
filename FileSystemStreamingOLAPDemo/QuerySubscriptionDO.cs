using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace RXDemo
{
    public class QuerySubscriptionDO : DependencyObject, IDisposable
    {

        private IDisposable _disp;

        public QuerySubscriptionDO(string queryName, IObservable<FileChangeFact> newQuerystream)
        {
            
            IObservable<StatInfoItem> subDirStats = newQuerystream.ToCommonAggregates(fcf => fcf.Length);

            IObservable<IGroupedObservable<string, long>> newQuerystreamGroupBy =
                    from item in newQuerystream
                    group item.Length by item.Extension;
                    

            IObservable<StatInfoItem<string>> grouped = 
                           from Ogrp in newQuerystreamGroupBy
                           from gag in Ogrp.ToCommonAggregates(x=> x, _ => Ogrp.Key)
                           select gag;



            //Wire up the UI
            
            GroupedByExtentionCollection = new ObservableCollection<StatInfoItem<string>>();

            //Set Name
            QueryName = queryName;

            // set agg summary            
            subDirStats.ObserveOnDispatcher().Subscribe(agg => TotalAggregate = agg);

            // update grouped items
            _disp = GroupedByExtentionCollection.MergeInsert(grouped, sii => sii.Item);

        }



        public void Dispose()
        {
            _disp.Dispose();
        }


        #region DP QueryName string
        public static readonly DependencyProperty QueryNameProperty = DependencyProperty.Register("QueryName", typeof(string), typeof(QuerySubscriptionDO), new UIPropertyMetadata(""));

        public string QueryName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(QueryNameProperty);
            }
            set
            {
                SetValue(QueryNameProperty, value);
            }
        }
        #endregion
        #region DP TotalAggregate PropType
        public static readonly DependencyProperty TotalAggregateProperty = DependencyProperty.Register("TotalAggregate", typeof(StatInfoItem), typeof(QuerySubscriptionDO), new UIPropertyMetadata(null));
        public StatInfoItem TotalAggregate
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (StatInfoItem)GetValue(TotalAggregateProperty);
            }
            set
            {
                SetValue(TotalAggregateProperty, value);
            }
        }
        #endregion

        public ObservableCollection<StatInfoItem<string>> GroupedByExtentionCollection { get; set; }

    }
}

//            IObservable<StatInfoItem<string>> grouped =  newQuerystreamGroupBy.SelectMany(grp => grp.ToCommonAggregates(x => x, _ => grp.Key));
