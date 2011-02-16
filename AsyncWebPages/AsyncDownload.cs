namespace RXDemo
{
    using RXDemo;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class AsyncDownload
    {
        public static void Main(string[] args)
        {
            // method 1 - all sync code
            Func<string, string> UrlToHtml = (url) =>
            {
                WebRequest wr = WebRequest.Create(url);
                using (StreamReader sr = new StreamReader(wr.GetResponse().GetResponseStream()))
               {
                   return sr.ReadToEnd();
               }
            };
            Func<string, IObservable<string>> xxx = Observable.ToAsync(UrlToHtml);

            // method 2 - using FromAsyncPattern
            Func<string, IObservable<string>> UrlToHTMLAsObservableString = (url) =>
                                                WebRequest.Create(url)
                                               .GetResponseAsync() // extention method 
                                               .Do(_ => Console.WriteLine(Thread.CurrentThread.ManagedThreadId))
                                               .Select((WebResponse wr) =>
                                                       {
                                                           using (var sr = new StreamReader(wr.GetResponseStream()))
                                                           {
                                                               return sr.ReadToEnd();
                                                           }
                                                       });

            // method 3 - fully non-blocking
            Func<string, IObservable<string>> UrlToHTMLAsObservableStringxs = (url) =>
                                                WebRequest.Create(url)
                                                .GetResponseAsync()
                                                .SelectMany(wr => wr.GetResponseStream().ToObservable())
                                                .Select(buff => UTF8Encoding.UTF8.GetString(buff));

            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            IObservable<string> urls = new string[] { "http://slate.com", "http://google.com", "http://bing.com", "http://weblogs.asp.net/" }
                                 .ToObservable();
                //.Repeat()
                //.Take(100)

            var r2 = from url in urls
                     let ostream = UrlToHTMLAsObservableString(url)
                     from str in ostream
                     select new { Url = url, HTML = str };
        
            var r3 = urls
                        .Select(url => new { _url = url, _ostream = UrlToHTMLAsObservableStringxs(url) })
                        .SelectMany((pair) => pair._ostream, (pair, html) => new { Url = pair._url, HTML = html });

           r3.Subscribe(pair => Console.WriteLine(pair.Url + pair.HTML));
           Console.WriteLine("press any key");
           Console.ReadKey();
        }
    }
}
