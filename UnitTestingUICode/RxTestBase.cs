namespace UnitTestingUICode
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Microsoft.Reactive.Testing;
    
    public abstract class RxTestBase
    {
        public static Recorded<Notification<T>> OnNext<T>(long ticks, T value)
        {
        
            return new Recorded<Notification<T>>(
                ticks,
                Notification.CreateOnNext(value));
        }

        public static Recorded<Notification<T>> OnNext<T>(TimeSpan time, T value)
        {
            return new Recorded<Notification<T>>(
                time.Ticks,
                Notification.CreateOnNext(value));
        }

        public static Recorded<Notification<T>> Value<T>(TimeSpan time, T value)
        {
            return new Recorded<Notification<T>>(
                time.Ticks,
                Notification.CreateOnNext(value));
        }

        public static Recorded<Notification<T>> OnCompleted<T>(long ticks)
        {
            return new Recorded<Notification<T>>(
                ticks,
                Notification.CreateOnCompleted<T>());
        }

        public static Recorded<Notification<T>> OnError<T>(long ticks, Exception exception)
        {
            return new Recorded<Notification<T>>(
                ticks,
                Notification.CreateOnError<T>(exception));
        }

        public static Subscription Subscribe(long start, long end)
        {
            return new Subscription(start, end);
        }

        public static Subscription Subscribe(long start)
        {
            return new Subscription(start);
        }
    }
}
