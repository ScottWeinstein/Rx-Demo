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
            //Traditional();
            //Rx();
            Console.ReadKey();
        }

        private static void Rx()
        {
            var ticks = Observable.Interval(TimeSpan.FromMilliseconds(10000), Scheduler.ThreadPool).Select(_ => "scheduled");
            var userRequests = Observable.Create<string>(subscribe: obs =>
                {
                    bool shouldRun = true;
                    while (shouldRun)
                    {
                        Console.ReadKey();
                        obs.OnNext("on demand");
                    }
                    return () => { shouldRun = false; };
                });

            Observable.Merge(ticks, userRequests)
                      .Throttle(TimeSpan.FromMilliseconds(500))
                      .Subscribe(GetSomeSlowData);
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
                GetSomeSlowDataTraditional("on demand");
            }
        }
        static volatile bool isRunning = false;
        private static void GetSomeSlowDataTraditional(string sender)
        {
            Action doBG = () =>
            {
                Console.WriteLine(sender);
                Thread.Sleep(1000);
                Console.WriteLine("Exit timer");
                isRunning = false;
            };
            if (!isRunning)
            {
                isRunning = true;
                Task.Factory.StartNew(doBG);
            }
        }

        
        private static void GetSomeSlowData(string sender)
        {
            Action doBG = () =>
            {
                Console.WriteLine(sender);
                Thread.Sleep(1000);
                Console.WriteLine("Exit timer");
                
            };
            Task.Factory.StartNew(doBG);
        }



    }
}
