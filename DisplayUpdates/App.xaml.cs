using System;
using System.Windows;
using Autofac;
using TweetSharp;
using System.Linq;

namespace DisplayUpdates
{
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var useTwitter = DisplayUpdates.Properties.Settings.Default.UseTwitter;

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(this.GetType().Assembly);

            builder.Register<Tuple<string,string>>(ctx => GetAuthKeys()).SingleInstance();
            
            //Observable.Never<TwitterStatus>()
            builder.Register<ITwitterFeed>(ctx => (useTwitter) ? (ITwitterFeed)ctx.Resolve<TwitterFeed>() : (ITwitterFeed) ctx.Resolve<FakeTwitterFeed>()).SingleInstance();
            builder.Register<IObservable<TwitterStatus>>(ctx => ctx.Resolve<ITwitterFeed>().Tweets);

            Container = builder.Build();
        }

        public Tuple<string,string> GetAuthKeys()
        {
            var lines = System.IO.File.ReadAllLines(@"..\..\DisplayUpdates\authkeys.txt").Select(item=> item.Trim());
            return new Tuple<string,string>(lines.First(),lines.Last());
        }

    }
}
