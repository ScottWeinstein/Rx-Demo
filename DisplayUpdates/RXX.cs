namespace DisplayUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;
    using System.Diagnostics;
    using System.Threading;

    public static class RXX
    {
        public static IObservable<T> ReplayLastByKey<T, TKey>(this IObservable<T> source, Func<T, TKey> keySelector)
        {
            var cache = new Dictionary<TKey, T>();
            return Observable
                .Defer(() => { lock (cache) return cache.Values.ToList().ToObservable(); })
                .Concat(source.Do(s => { lock (cache) cache[keySelector(s)] = s; }));
        }

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
    }
}
