using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Concurrency;

namespace UnitTestingUICode
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new EventHandler(timer_Tick);
            for (int ii = 0; ii < 9; ii++)
            {
            	var btn = new Button() { Content = ii };
                this.KeyPadGrid.Children.Add(btn);
                //btn.Click += (sender, e) =>
                //{
                //    timer.Stop();
                //    EnteredPassKey += ((Button)sender).Content.ToString();
                //    CheckPasskey(EnteredPassKey);
                //    timer.Start();
                //};
            }

            IObservable<string> keypresses =  this.KeyPadGrid.Children.OfType<Button>()
                                    .Select(btn => Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
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
        
        public IObservable<bool> DetectCorrectKeypass(IObservable<string> keypresses, string password, TimeSpan delay)
        {
            return DetectCorrectKeypass(keypresses, password, delay, Scheduler.ThreadPool);
        }
        public IObservable<bool> DetectCorrectKeypass(IObservable<string> keypresses, string password, TimeSpan delay, IScheduler scheduler)
        {
            return keypresses.BufferWithTimeOrCount(delay, password.Length, scheduler)
                            .Select(listStr => string.Join("",listStr.ToArray()))
                           // .SelectMany(a => a.Aggregate("", (acc, curr) => acc += curr))
                            .Where(guess => guess != "")
                            .Select(guess => guess == password)
                            .DistinctUntilChanged();
        }
        public bool CheckPasskey(string enteredPassKey)
        {
            IsCorrectPassKey = enteredPassKey == "12345";
            return IsCorrectPassKey;
        }

        #region DP IsCorrectPassKey bool
        public static readonly DependencyProperty IsCorrectPassKeyProperty = DependencyProperty.Register("IsCorrectPassKey", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));

        public bool IsCorrectPassKey
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
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
        
        void timer_Tick(object sender, EventArgs e)
        {
            EnteredPassKey = string.Empty;    
        }

    }
}
