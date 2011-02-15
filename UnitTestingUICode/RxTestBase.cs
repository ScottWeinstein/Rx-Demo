using System;
using System.Collections.Generic;
using System.Reactive.Testing;

namespace UnitTestingUICode
{
    public abstract class RxTestBase
    {
        public Recorded<Notification<T>> OnNext<T>(long ticks, T value)
        {
            return new Recorded<Notification<T>>(
                ticks, 
                new Notification<T>.OnNext(value));
        }

        public Recorded<Notification<T>> OnNext<T>(TimeSpan time, T value)
        {
            return new Recorded<Notification<T>>(
                time.Ticks, 
                new Notification<T>.OnNext(value));
        }
        public Recorded<Notification<T>> Value<T>(TimeSpan time, T value)
        {
            return new Recorded<Notification<T>>(
                time.Ticks, 
                new Notification<T>.OnNext(value));
        }


        public Recorded<Notification<T>> OnCompleted<T>(long ticks)
        {
            return new Recorded<Notification<T>>(
                ticks, 
                new Notification<T>.OnCompleted());
        }

        public Recorded<Notification<T>> OnError<T>(long ticks, Exception exception)
        {
            return new Recorded<Notification<T>>(
                ticks,
                new Notification<T>.OnError(exception));
        }

        public Subscription Subscribe(long start, long end)
        {
            return new Subscription(start, end);
        }

        public Subscription Subscribe(long start)
        {
            return new Subscription(start);
        }
    }
}
