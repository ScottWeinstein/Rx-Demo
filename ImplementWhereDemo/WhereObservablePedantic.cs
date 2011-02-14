using System;
using System.Linq;

namespace RXDemo
{
    public class WhereObservablePedantic<T> : IObservable<T>
    {

        private Func<T, bool> _pred;
        private IObservable<T> _stream;
        public WhereObservablePedantic(IObservable<T> stream, Func<T, bool> pred)
        {
            _pred = pred;
            _stream = stream;
        }

        public IDisposable Subscribe(IObserver<T> downStreamObserver)
        {
            return _stream.Subscribe(new WhereObserverPedantic<T>(downStreamObserver, _pred));
        }
    }
}
