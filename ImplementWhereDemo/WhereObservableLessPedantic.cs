using System;
using System.Linq;

namespace RXDemo
{
    public class WhereObservableLessPedantic<T> : IObservable<T>
    {

        private Func<T, bool> _pred;
        private IObservable<T> _stream;
        public WhereObservableLessPedantic(IObservable<T> stream, Func<T, bool> pred)
        {
            _pred = pred;
            _stream = stream;
        }

        public IDisposable Subscribe(IObserver<T> downStreamObserver)
        {
            Action<T> onNext = nextVal =>
            {
                if (_pred(nextVal))
                    downStreamObserver.OnNext(nextVal);
            };
            return _stream.Subscribe(onNext);
        }
    }
}
