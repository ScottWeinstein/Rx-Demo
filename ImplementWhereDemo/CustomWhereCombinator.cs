using System;
using System.Linq;
using System.Collections.Generic;

namespace RXDemo
{
    public static class CustomWhereCombinator
    {

        public static IObservable<T> CustomWherePedantic<T>(this IObservable<T> stream, Func<T, bool> pred)
        {
            return new WhereObservablePedantic<T>(stream, pred);
        }

        public static IObservable<T> CustomWhereLessPedantic<T>(this IObservable<T> stream, Func<T, bool> pred)
        {
            return new WhereObservableLessPedantic<T>(stream, pred);
        }

        public static IObservable<T> CustomWhere<T>(this IObservable<T> stream, Func<T, bool> pred)
        {
            return Observable.CreateWithDisposable<T>(downStreamObs => stream.Subscribe(
                                    val =>
                                    {
                                        if (pred(val))
                                        {
                                            downStreamObs.OnNext(val);
                                        }
                                    }));
        }


        public static IEnumerable<T> CustomWhere<T>(this IEnumerable<T> source, Predicate<T> pred)
        {
            foreach (T item in source)
            {
                if (pred(item))
                    yield return item;
            }
        }

        public static IEnumerable<T> CustomWhereNoYield<T>(this IEnumerable<T> source, Predicate<T> pred)
        {
            return new WhereEnumerable<T>(source, pred);
        }



    }

}