using System;
using System.Windows;
using Autofac;
using System.Diagnostics;
using System.Threading;

namespace DisplayUpdates
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Container.Resolve<TwitterFeedVM>();
        }
        
    }
}
