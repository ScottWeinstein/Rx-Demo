using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.Disposables;
using System.Concurrency;

namespace PollingWS
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsWL(ConsoleColor.White, "" + Thread.CurrentThread.ManagedThreadId);
            Traditional();
            //Rx(RxTicks,RxUserKeys,TimeSpan.FromMilliseconds(500));
            Console.ReadKey();
        }

        private static void Rx(IObservable<string> rxTicks, IObservable<string> rxUserKeys, TimeSpan ts)
        {
            Observable.Merge(rxTicks, rxUserKeys)
                      .Throttle(ts)
                      .Subscribe(GetSomeSlowData);
        }

        private static IObservable<string> RxUserKeys
        {
            get
            {
                return Observable.Create<string>(subscribe: obs =>
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
        private static IObservable<string> RxTicks
        {
            get
            {
                return Observable.Interval(TimeSpan.FromMilliseconds(10000)).Select(_ => "scheduled");
            }
        }


        private static void Traditional()
        {
            var timer = new System.Timers.Timer(10000);
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
                Task.Factory.StartNew(() => GetSomeSlowData(sender))
                    .ContinueWith(tsk => { isRunning = false; });
            }
        }

        
        private static void GetSomeSlowData(string sender)
        {
            ConsWL(ConsoleColor.Green,sender);
            Thread.Sleep(3000);
            ConsWL(ConsoleColor.Yellow,"\n{0} Exit timer",Thread.CurrentThread.ManagedThreadId);    
        }

        private static void ConsWL(ConsoleColor color, string ftm,params object[] args)
        {
            using (ChangeConsoleColor(color))
            {
                Console.WriteLine(ftm,args);
            }
        }
        public static IDisposable ChangeConsoleColor(ConsoleColor newcolor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = newcolor;
            return Disposable.Create(() => Console.ForegroundColor = color);
        }
        



    }
}
