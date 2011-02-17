namespace RXDemo
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class WhereEnumerator<T> : IEnumerator<T>
    {
        private Predicate<T> _pred;
        //private IEnumerable<T> _source;
        private IEnumerator<T> _srcEnum;
        private T _current;
        public WhereEnumerator(IEnumerable<T> source, Predicate<T> pred)
        {
            _pred = pred;
            _srcEnum = source.GetEnumerator();
        }

        public T Current
        {
            get { return _srcEnum.Current; }
        }

        public void Dispose()
        {
            _srcEnum.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            while (_srcEnum.MoveNext())
            {
                _current = _srcEnum.Current;
                if (_pred(_current))
                    return true;
            }

            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
