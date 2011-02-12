using System;
using System.Concurrency;
using System.Reactive.Testing;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace UnitTestingUICode
{
    public class Tests: RxTestBase
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
            var ioKeys = _scheduler.CreateHotObservable(OnNext(210, "1"), OnNext(220, "2"), OnNext(230, "3"), OnNext(240, "4"));
            // Act
            IEnumerable<Recorded<Notification<bool>>> actuals = _scheduler.Run( () => _window.DetectCorrectKeypass(ioKeys, "1234", TimeSpan.FromTicks(50), _scheduler) );
            // Assert
            actuals.AssertEqual(OnNext(240, true));
        }

        [Fact]
        public void Wrong_password_is_rejected()
        {
            // setup
            var ioKeys = _scheduler.CreateHotObservable(OnNext(210, "1"), OnNext(220, "2"), OnNext(230, "3"), OnNext(240, "4"));
            // Act
            IEnumerable<Recorded<Notification<bool>>> actuals = _scheduler.Run(() => _window.DetectCorrectKeypass(ioKeys, "ABCD", TimeSpan.FromTicks(50), _scheduler));
            // Assert
            actuals.AssertEqual(OnNext(240, false));
        }

        [Fact]
        public void Lazy_typist_is_Rejected()
        {
            // setup
            var ioKeys = _scheduler.CreateHotObservable(OnNext(210, "1"), OnNext(220, "2"), OnNext(230, "3"), OnNext(340, "4"));
            // Act
            IEnumerable<Recorded<Notification<bool>>> actuals = _scheduler.Run(() => _window.DetectCorrectKeypass(ioKeys, "1234", TimeSpan.FromTicks(50), _scheduler));
            // Assert
            actuals.AssertEqual(OnNext(250, false));
        }
    }
}
