namespace RXDemo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class RXX
    {
        public static IDisposable MergeInsert<T, TKey>(this ObservableCollection<T> col, IObservable<T> stream, Func<T, TKey> keySelector)
        {
            col.Clear();
            Dictionary<TKey, int> lookupTable = new Dictionary<TKey, int>();
            return stream.ObserveOnDispatcher().Subscribe(item =>
            {
                var key = keySelector(item);
                int index;
                if (!lookupTable.TryGetValue(key, out index))
                {
                    lookupTable[key] = col.Count;
                    col.Add(item);
                }
                else
                {
                    col[index] = item;
                }
            });
        }

        public static IDisposable Append<T>(this ObservableCollection<T> col, IObservable<T> stream)
        {
            return stream.ObserveOnDispatcher().Subscribe(item =>
            {
                col.Add(item);
            });
        }

        public static IDisposable Insert<T>(this ObservableCollection<T> col, IObservable<T> stream)
        {
            return stream.ObserveOnDispatcher()
                .Subscribe(item =>
                    {
                        col.Insert(0, item);
                    });
        }

        public static IObservable<double> StdDev(this IObservable<double> source)
        {
            var temp = new { N = 0, Mean = 0d, M2 = 0d };
            return source.Scan(temp, (cur, next) =>
            {
                var n = cur.N + 1;
                var delta = next - cur.Mean;
                var meanp = cur.Mean + delta / n;
                return new
                {
                    N = n,
                    Mean = meanp,
                    M2 = cur.M2 + delta * (next - meanp)
                };
            }).Select(it => Math.Sqrt(it.M2 / it.N));
        }

        public static IObservable<double> Mean(this IObservable<double> source)
        {
            var temp = new { N = 0, Mean = 0d };
            return source.Scan(temp, (cur, next) =>
            {
                var n = cur.N + 1;
                var delta = next - cur.Mean;
                var meanp = cur.Mean + delta / n;
                return new
                {
                    N = n,
                    Mean = meanp,
                };
            }).Select(it => it.Mean);
        }
    }
}
