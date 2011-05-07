namespace DisplayUpdates
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Autofac;
    using DisplayUpdates.Properties;
    using TweetSharp;

    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var useTwitter = Settings.Default.UseTwitter;

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(GetType().Assembly);
            builder.Register<Tuple<string, string>>(ctx => GetAuthKeys()).SingleInstance();

            //Observable.Never<TwitterStatus>()
            builder.Register<ITwitterFeed>(ctx => OnStartupExtracted(ctx, useTwitter)).SingleInstance();
            builder.Register<IObservable<TwitterStatus>>(ctx => ctx.Resolve<ITwitterFeed>().Tweets);

            Container = builder.Build();
        }

        private static ITwitterFeed OnStartupExtracted(IComponentContext ctx, bool useTwitter)
        {
            if (useTwitter)
                return ctx.Resolve<TwitterFeedAsync>();
            else
                return ctx.Resolve<FakeTwitterFeed>();
        }

        public static Tuple<string, string> GetAuthKeys()
        {
            var lines = File.ReadAllLines(@"..\..\DisplayUpdates\authkeys.txt")
                            .Select(item => item.Trim());
            
            return new Tuple<string, string>(lines.First(), lines.Last());
        }
    }
}
