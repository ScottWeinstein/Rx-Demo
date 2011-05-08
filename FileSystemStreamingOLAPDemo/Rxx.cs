namespace RXDemo
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive;
    using System.Reactive.Linq;

    public static class Rxx
    {
        public static IObservable<StatInfoItem> ToCommonAggregates<TSrc>(this IObservable<TSrc> source, Func<TSrc, double> dataSelector)
        {
            return ToCommonAggregates(source, dataSelector, _ => Unit.Default);
        }

        public static IObservable<StatInfoItem<T>> ToCommonAggregates<T, TSrc>(this IObservable<TSrc> source, Func<TSrc, double> dataSelector, Func<TSrc, T> itemSelector)
        {
            return source.Scan(new StatInfoItem<T>(), (cur, next) =>
            {
                double data = dataSelector(next);
                T itemp = itemSelector(next);
                var n = cur.Count + 1;
                var delta = data - cur.Mean;
                var meanp = cur.Mean + delta / n;
                var m2 = cur.M2 + delta * (data - meanp);
                var stdDevp = Math.Sqrt(m2 / n);
                return new StatInfoItem<T>()
                {
                    Item = itemp,
                    Sum = data + cur.Sum,
                    Count = n,
                    Mean = meanp,
                    M2 = m2,
                    StdDev = stdDevp,
                    Min = Math.Min(data, cur.Min),
                    Max = Math.Max(data, cur.Max),
                };
            })
            .Skip(1); // need a seed, but don't want to include seed value in the output
        }
    }
}
