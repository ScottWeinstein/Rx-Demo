namespace UnitTestingUICode
{
    using System;
    using Microsoft.Reactive.Testing;
    using Xunit;

    public class Tests : RxTestBase
    {
        private readonly MainWindow _window;
        private readonly TestScheduler _scheduler;
        
        public Tests()
        {
            _window = new UnitTestingUICode.MainWindow();
            _scheduler = new TestScheduler();
        }

        [Fact]
        public void Correct_password_is_accepted()
        {
            // setup
            var ioKeys = _scheduler.CreateHotObservable(
                             OnNext(210, "1"), 
                             OnNext(220, "2"), 
                             OnNext(230, "3"), 
                             OnNext(240, "4"));

            Func<IObservable<bool>> target = () => 
                _window.DetectCorrectKeypass(
                    ioKeys, 
                    "1234", 
                    TimeSpan.FromTicks(50), 
                    _scheduler);

            // Act
            ITestableObserver<bool> actuals = _scheduler.Start(target);
            
            // Assert
            ReactiveAssert.AreElementsEqual(new[] { OnNext(240, true) }, actuals.Messages);
        }

        [Fact]
        public void Wrong_password_is_rejected()
        {
            // setup
            var ioKeys = _scheduler.CreateHotObservable(
                             OnNext(210, "1"), 
                             OnNext(220, "2"), 
                             OnNext(230, "3"), 
                             OnNext(240, "4"));
            Func<IObservable<bool>> target = () => _window.DetectCorrectKeypass(
                                                       ioKeys, 
                                                       "XXXX", 
                                                       TimeSpan.FromTicks(50), 
                                                       _scheduler);
            
            // Act
            var actuals = _scheduler.Start(target);
            
            // Assert
            ReactiveAssert.AreElementsEqual(new [] { OnNext(240, false) }, actuals.Messages);
        }

        [Fact]
        public void Lazy_typist_is_Rejected()
        {
            // setup
            var ioKeys = _scheduler.CreateHotObservable(
                             OnNext(210, "1"), 
                             OnNext(220, "2"), 
                             OnNext(230, "3"), 
                             OnNext(340, "4"));
            Func<IObservable<bool>> target = () => _window.DetectCorrectKeypass(
                                                       ioKeys, 
                                                       "1234", 
                                                       TimeSpan.FromTicks(50), 
                                                       _scheduler);
            
            // Act
            var actuals = _scheduler.Start(target);
            
            // Assert
            ReactiveAssert.AreElementsEqual(new [] { OnNext(250, false) }, actuals.Messages);
        }
    }
}
