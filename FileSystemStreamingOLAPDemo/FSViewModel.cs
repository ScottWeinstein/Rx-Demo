namespace RXDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reactive.Subjects;
    using System.Reactive;
    using System.Reactive.Linq;

    public class FSViewModel : DependencyObject
    {
        public ObservableCollection<FileChangeFact> FSItems { get; set; }
        public ObservableCollection<QuerySubscriptionDO> QuerySubscriptions { get; set; }

        private IConnectableObservable<FileChangeFact> _storeSubject;

        public FSViewModel(IObservable<EventPattern<RoutedEventArgs>> uiAddQuery)
        {
            _storeSubject = GetFileSystemStream().Replay();

            //Compute aggregates  - 1st attempt
            IObservable<double> fileLenthgs = _storeSubject.Select(fcf => (double)fcf.Length);

            IObservable<int> fileCounts = fileLenthgs.Scan(0, (count, _) => count + 1);

            IObservable<double> fileLenRunningSums = fileLenthgs.Scan((sum, next) => sum + next);
            
            IObservable<FileChangeAggregate> aggCountSum =
                fileCounts.Zip(fileLenRunningSums, (cnt, sum) => new FileChangeAggregate() { Sum = sum, Count = cnt });
            
            IObservable<FileChangeAggregate> aggCountSumMean =
                aggCountSum.Zip(fileLenthgs.Mean(), (fca, mean) => { fca.Mean = mean; return fca; });
            
            IObservable<FileChangeAggregate> aggCountSumMeanStdDev = 
                aggCountSumMean.Zip(fileLenthgs.StdDev(), (fca, stddev) => { fca.StdDev = stddev; return fca; });

            uiAddQuery.Subscribe(_ignore =>
            {
                Func<FileChangeFact, bool> fltr = _ => true;
                if (!String.IsNullOrEmpty(FilterPath))
                {
                    string fp = FilterPath; // need to copy the value from the UI based DP so that the RX based Where() clause can safely access it
                    fltr = fci => fci.Path.StartsWith(fp, StringComparison.CurrentCultureIgnoreCase);
                }

                QuerySubscriptions.Add(new QuerySubscriptionDO(FilterPath, _storeSubject.Where(fltr)));
            });

            //Wire up UI
            FSItems = new ObservableCollection<FileChangeFact>();
            QuerySubscriptions = new ObservableCollection<QuerySubscriptionDO>();
            aggCountSumMeanStdDev.ObserveOnDispatcher().Subscribe(fca => TotalAggregate = fca); 
            FSItems.Insert(_storeSubject);

            _storeSubject.Connect();
        }

        public static IObservable<FileChangeFact> GetFileSystemStream()
        {
            IEnumerable<FileSystemWatcher> seqFSWatchers =
                        from drive in DriveInfo.GetDrives()
                        where drive.DriveType == DriveType.Fixed
                        select new FileSystemWatcher(drive.RootDirectory.FullName) 
                                { 
                                    IncludeSubdirectories = true, 
                                    EnableRaisingEvents = true 
                                };
            
            //notice the SelectMany()
            IEnumerable<IObservable<EventPattern<FileSystemEventArgs>>> seqfsEventsAsObservables = 
                        from FSWatcher in seqFSWatchers
                        from eventType in new string[] { "Changed", "Deleted", "Created" }
                        select Observable.FromEventPattern<FileSystemEventArgs>(FSWatcher, eventType);

            IObservable<EventPattern<FileSystemEventArgs>> fsEventsMerged = Observable.Merge(seqfsEventsAsObservables);

            IObservable<FileChangeFact> fsChanges = fsEventsMerged
                            .Select(fsea =>
                            {
                                var fi = new FileInfo(fsea.EventArgs.FullPath);
                                return new FileChangeFact
                                {
                                    ChangeType = fsea.EventArgs.ChangeType,
                                    Path = fsea.EventArgs.FullPath,
                                    IsContainer = !fi.Exists,
                                    Length = fi.Exists ? fi.Length : 0,
                                    Extension = String.IsNullOrEmpty(fi.Extension) ? "(none)" : fi.Extension
                                };
                            });
            return fsChanges;
        }
 
        #region DP FilterPath string
        public static readonly DependencyProperty FilterPathProperty = DependencyProperty.Register("FilterPath", typeof(string), typeof(FSViewModel), new UIPropertyMetadata(string.Empty));

        public string FilterPath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(FilterPathProperty);
            }

            set
            {
                SetValue(FilterPathProperty, value);
            }
        }
        #endregion

        #region DP TotalAggregate FileChangeAggregate
        public static readonly DependencyProperty TotalAggregateProperty = DependencyProperty.Register("TotalAggregate", typeof(FileChangeAggregate), typeof(FSViewModel), new UIPropertyMetadata(null));

        public FileChangeAggregate TotalAggregate
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FileChangeAggregate)GetValue(TotalAggregateProperty);
            }

            set
            {
                SetValue(TotalAggregateProperty, value);
            }
        }
        #endregion
    }
}
//IEnumerable<IObservable<IEvent<FileSystemEventArgs>>> fsEventsAsObservables =
//    seqFSWatchers
//            .SelectMany(FSWatcher => fsEventTypes.Select(eventType => Observable.FromEvent<FileSystemEventArgs>(FSWatcher, eventType)));
