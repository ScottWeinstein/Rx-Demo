namespace DisplayUpdates
{
    using System;
    using System.Windows;
    using Autofac;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Container.Resolve<TwitterFeedVM>();
        }
    }
}
