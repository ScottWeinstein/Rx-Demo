namespace RXDemo
{
    using System;
    using System.Windows;
    using System.Linq;
    using System.Reactive.Linq;

    public partial class FS : Window
    {
        public FS()
        {
            InitializeComponent();
            DataContext = new FSViewModel(Observable.FromEventPattern<RoutedEventArgs>(filterBtn, "Click"));
        }
    }
}
