using System;
using System.Windows;
using System.Linq;

namespace RXDemo
{
    public partial class FS : Window
    {

        public FS()
        {
            InitializeComponent();
            DataContext = new FSViewModel(Observable.FromEvent<RoutedEventArgs>(filterBtn, "Click"));
            
        }

    }
}
