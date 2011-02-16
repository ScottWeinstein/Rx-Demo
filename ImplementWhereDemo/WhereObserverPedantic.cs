namespace RXDemo
{
    using System;
    using System.Linq;

    public class WhereObserverPedantic<T> : IObserver<T>
    {
        private IObserver<T> _downStreamObserver;
        private Func<T, bool> _pred;

        public WhereObserverPedantic(IObserver<T> downStreamObserver, Func<T, bool> pred)
        {
            _pred = pred;
            _downStreamObserver = downStreamObserver;
        }

        public void OnNext(T value)
        {
            if (_pred(value))
            {
                _downStreamObserver.OnNext(value);
            }
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
    }
}
