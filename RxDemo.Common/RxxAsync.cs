namespace RXDemo
{
    using System;
    using System.Linq;
    using System.Net;
    using System.IO;
    using System.Reactive.Linq;

    public static class RxxAsync
    {
        public static IObservable<WebResponse> GetResponseAsync(this WebRequest webrequest)
        {
            return Observable.FromAsyncPattern<WebResponse>(webrequest.BeginGetResponse, webrequest.EndGetResponse)();
        }

        public static IObservable<byte[]> ToAsync(this Stream source)
        {
            int size = 1024 * 8;
            byte[] buff = new byte[size];
            IObservable<int> res = Observable.FromAsyncPattern<byte[], int, int, int>(
                                                source.BeginRead,
                                                source.EndRead)(buff, 0, size);

            return res.Select(ii => (ii < size) ? buff.Take(ii).ToArray() : buff);
        }

        public static IObservable<byte[]> ToObservable(this Stream source)
        {
            return Observable.Generate(source.ToAsync(),
                                                Isrr => Isrr.First().Count() > 0,
                                                _ => _,
                                                _ => source.ToAsync())
                                                .SelectMany(x => x);
        }
    }
}
