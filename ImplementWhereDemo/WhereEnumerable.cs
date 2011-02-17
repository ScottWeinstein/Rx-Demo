namespace RXDemo
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class WhereEnumerable<T> : IEnumerable<T>
    {
        private Predicate<T> _p;
        private IEnumerable<T> _s;
        public WhereEnumerable(IEnumerable<T> source, Predicate<T> pred)
        {
            _s = source;
            _p = pred;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new WhereEnumerator<T>(_s, _p);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
