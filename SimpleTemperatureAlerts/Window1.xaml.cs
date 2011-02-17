namespace RXDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Collections.ObjectModel;

    public partial class Window1 : Window
    {
        public Window1()
        {
            //Setup twitter
            //IFluentTwitter twitclient = FluentTwitter.CreateRequest().AuthenticateAs("uid", "pwd");
            //ITwitterStatuses twitStatuses = twitclient.Statuses();

            //generate temperature alerts
            ObservableUsbTemper temperature = new ObservableUsbTemper();

            IObservable<double> ts = temperature.TemperatureStreamFarenheight;
            IObservable<IList<double>> tsSlidingWindow = ts.BufferWithTime(TimeSpan.FromSeconds(5));
            IObservable<double> avgTempOverTime = tsSlidingWindow.Select(buff => buff.Average());
            IObservable<double> tempAlerts = avgTempOverTime.Where(avgtemp => avgtemp < 80);

            // Send to twitter
            IObservable<string> twitterResponses = tempAlerts.Sample(TimeSpan.FromMinutes(1))
                .Select(cold => string.Format("It's cold here. Avg temp is {0:0.00}", cold));

            // now wire-up the UI
            InitializeComponent();
            this.DataContext = this;

            OCFarenheightReadings = new ObservableCollection<double>();
            OCSlidingWindow = new ObservableCollection<IList<double>>();
            OCavgTempOverTime = new ObservableCollection<double>();
            OCtempAlerts = new ObservableCollection<double>();
            OCtwitterResponses = new ObservableCollection<string>();

            OCFarenheightReadings.Insert(ts);
            OCSlidingWindow.Insert(tsSlidingWindow);
            OCavgTempOverTime.Insert(avgTempOverTime);
            OCtempAlerts.Insert(tempAlerts);
            OCtwitterResponses.Insert(twitterResponses);
        }

        public ObservableCollection<double> OCFarenheightReadings { get; set; }
        public ObservableCollection<IList<double>> OCSlidingWindow { get; set; }
        public ObservableCollection<double> OCavgTempOverTime { get; set; }
        public ObservableCollection<double> OCtempAlerts { get; set; }
        public ObservableCollection<string> OCtwitterResponses { get; set; }
    }
}
