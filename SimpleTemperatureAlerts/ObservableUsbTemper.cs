namespace RXDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Disposables;
    using System.Concurrency;
    using System.Windows.Media;

    public class ObservableUsbTemper : IDisposable
    {
        private IDisposable _disp;
        public ObservableUsbTemper()
        {
            UsbTEMPer[] devices = UsbTEMPer.FindDevices()
                                            .Select((port, idx) => new UsbTEMPer(idx))
                                            .ToArray();
            _disp = new CompositeDisposable(devices);
            var thermometer = devices.First();

            var txs = Observable.Generate(thermometer.GetTemperature(), _ => true, t => t, t => thermometer.GetTemperature(), Scheduler.ThreadPool)
                .Replay(1);
            TemperatureStreamCelcius = txs;
            TemperatureStreamFarenheight = txs.Select(t => (t * 9 / 5) + 32);
            txs.Connect();
        }

        public IObservable<double> TemperatureStreamCelcius { get; private set; }
        public IObservable<double> TemperatureStreamFarenheight { get; private set; }

        public void Dispose()
        {
            _disp.Dispose();
        }
    }
}
