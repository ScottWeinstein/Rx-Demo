using System;
using System.Windows;
using Autofac;

namespace DisplayUpdates
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.Container.Resolve<TwitterFeedVM>();
        }
        
    }
}
