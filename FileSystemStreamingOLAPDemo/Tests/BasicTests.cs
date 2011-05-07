namespace RXDemo.Tests
{
    using System;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using Xunit;
    using Xunit.Extensions;
    
    public class BasicTests
    {
      [Theory, ExcelData(@"Tests\TestData.xls", "select * from StdDevData")]
        public void TestStdDev1(double expected, double d1, double d2, double d3, double d4)
        {
            double[] data = new double[] { d1, d2, d3, d4 };
            var actual =
                data.ToObservable().StdDev().Last();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGroupingAndAggregates()
        {
            var actual = Observable.Return(
                new FileChangeFact()
                {
                    ChangeType = System.IO.WatcherChangeTypes.Changed,
                    Extension = ".none",
                    IsContainer = false,
                    Length = 1,
                    Path = string.Empty
            }).Repeat(2)
            .ToCommonAggregates(fcf => fcf.Length, fcf => fcf.Extension)
            .ToEnumerable();

            Assert.Equal(1, actual.Count());
        }

        [Fact(Timeout = 400)] 
        public void CanUnsubscribe()
        {
            var infinite = Observable.Return(new { Name = "test", Value = 0d }, Scheduler.TaskPool).Repeat();
            var grouped = infinite.GroupBy(x => x.Name, x => x.Value);
            var disp = grouped.Subscribe(_ => { });
            Thread.Sleep(5);
            
            disp.Dispose();

            disp = grouped.Subscribe(_ => { });
            Thread.Sleep(5);
            
            disp.Dispose();
        }
    }
}
