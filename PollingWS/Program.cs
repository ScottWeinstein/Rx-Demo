using System;
using System.Disposables;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PollingWS
{
    class Program
    {
        static void Main()
        {
            //Traditional();
            Rx(RxUserKeys,TimeSpan.FromMilliseconds(500));
            Console.ReadKey();
        }

        #region RX
        private static void Rx(IObservable<string> rxUserKeys, TimeSpan ts)
        {
            var rxTicks = Observable.Interval(TimeSpan.FromMilliseconds(20000))
                                    .Select(_ => "scheduled");
            
            Observable.Merge(rxTicks, rxUserKeys)
                      .Throttle(ts)
                      .Select(a => GetSomeSlowData(a))
                      .Subscribe(Console.WriteLine);
        }

        private static IObservable<string> RxUserKeys
        {
            get
            {
                return Observable.Create<string>(obs =>
                                {
                                    bool shouldRun = true;
                                    while (shouldRun)
                                    {
                                        Console.ReadKey();
                                        obs.OnNext("ButtonClick");
                                    }
                                    return () => { shouldRun = false; };
                                });
            }
        }
        
        #endregion

        #region Traditional
        private static void Traditional()
        {
            var timer = new System.Timers.Timer(20000);
            timer.Elapsed += (sender, e) =>
            {
                GetSomeSlowDataTraditional("scheduled");
            };
            timer.Enabled = true;

            while (true)
            {
                Console.ReadKey();
                GetSomeSlowDataTraditional("ButtonClick");
            }
        }

        static volatile bool isRunning = false;
        private static void GetSomeSlowDataTraditional(string sender)
        {
            if (!isRunning)
            {
                isRunning = true;
                Task.Factory.StartNew<int>(() => GetSomeSlowData(sender))
                    .ContinueWith(tsk => { isRunning = false; });
            }
        }
        #endregion

        #region Commmon Helpers

        private static int GetSomeSlowData(string sender)
        {
            ConsWL(ConsoleColor.Green, sender);
            Thread.Sleep(3000);
            ConsWL(ConsoleColor.Yellow, "\n{0} Exit timer", Thread.CurrentThread.ManagedThreadId);
            return 42; ;
        }

        private static void ConsWL(ConsoleColor color, string ftm, params object[] args)
        {
            using (ChangeConsoleColor(color))
            {
                Console.WriteLine(ftm, args);
            }
        }
        public static IDisposable ChangeConsoleColor(ConsoleColor newcolor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = newcolor;
            return Disposable.Create(() => Console.ForegroundColor = color);
        }
        #endregion
    }
}
