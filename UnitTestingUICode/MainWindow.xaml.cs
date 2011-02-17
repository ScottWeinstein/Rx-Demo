namespace UnitTestingUICode
{
    using System;
    using System.Concurrency;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            bool useRx = true;
            DataContext = this;
            InitializeComponent();

            #region Traditional
            DispatcherTimer timer = null;
            if (!useRx)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += timer_Tick;
            }
            #endregion

            for (int ii = 0; ii < 9; ii++)
            {
                var btn = new Button() { Content = ii };
                this.KeyPadGrid.Children.Add(btn);

                #region Traditional
                if (!useRx)
                {
                    btn.Click += (sender, e) =>
                    {
                        timer.Stop();
                        EnteredPassKey += ((Button)sender).Content.ToString();
                        CheckPasskey(EnteredPassKey);
                        timer.Start();
                    };
                }
                #endregion
            }

            #region RX
            if (useRx)
            {
                IObservable<string> keypresses =
                        KeyPadGrid.Children.OfType<Button>()
                        .Select(btn => 
                            Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                                        h => new RoutedEventHandler(h),
                                        h => btn.Click += h,
                                        h => btn.Click -= h))
                                    .Merge()
                                    .Select(ireh => ireh.Sender)
                                    .OfType<Button>()
                                    .Select(btn => btn.Content.ToString());

                DetectCorrectKeypass(keypresses, "1234", TimeSpan.FromSeconds(5))
                    .ObserveOnDispatcher()
                    .Subscribe(results => { IsCorrectPassKey = results; });
            }
            #endregion
        }

        #region RX

        public IObservable<bool> DetectCorrectKeypass(IObservable<string> keypresses, string password, TimeSpan delay)
        {
            return DetectCorrectKeypass(
                       keypresses, 
                       password, 
                       delay, 
                       Scheduler.Dispatcher);
        }

        public IObservable<bool> DetectCorrectKeypass(IObservable<string> keypresses, string password, TimeSpan delay, IScheduler scheduler)
        {
            return keypresses
                .BufferWithTimeOrCount(delay, password.Length, scheduler)
                .Select(listStr => string.Join(string.Empty, listStr.ToArray()))
                .Do(guess => EnteredPassKey = guess)
                .Where(guess => guess != string.Empty)
                .Select(guess => guess == password)
                .DistinctUntilChanged();
        }
        #endregion
        
        #region Traditional

        public bool CheckPasskey(string enteredPassKey)
        {
            IsCorrectPassKey = enteredPassKey == "1234";
            return IsCorrectPassKey;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            EnteredPassKey = string.Empty;
        }
        #endregion

        #region DP IsCorrectPassKey bool
        public static readonly DependencyProperty IsCorrectPassKeyProperty = DependencyProperty.Register("IsCorrectPassKey", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));

        public bool IsCorrectPassKey
        {
            get
            {
                return (bool)GetValue(IsCorrectPassKeyProperty);
            }

            set
            {
                SetValue(IsCorrectPassKeyProperty, value);
            }
        }
        #endregion

        #region DP EnteredPassKey string
        public static readonly DependencyProperty EnteredPassKeyProperty = DependencyProperty.Register("EnteredPassKey", typeof(string), typeof(MainWindow), new UIPropertyMetadata(string.Empty));

        public string EnteredPassKey
        {
            get
            {
                return (string)GetValue(EnteredPassKeyProperty);
            }

            set
            {
                SetValue(EnteredPassKeyProperty, value);
            }
        }
        #endregion
    }
}
