using Clean_Reader.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class ColorDisplayBlock : UserControl
    {
        public event EventHandler<ColorEventArgs> Apply;
        public event EventHandler<ColorEventArgs> Delete;
        public ColorDisplayBlock()
        {
            this.InitializeComponent();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarButton;
            switch (btn.Label)
            {
                case "Apply":
                    Apply?.Invoke(this, new ColorEventArgs(this.DataContext as ReaderColorConfig));
                    break;
                case "Delete":
                    Delete?.Invoke(this, new ColorEventArgs(this.DataContext as ReaderColorConfig));
                    break;
                default:
                    break;
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Apply?.Invoke(this, new ColorEventArgs(this.DataContext as ReaderColorConfig));
        }
    }

    public class ColorEventArgs : EventArgs
    {
        public ReaderColorConfig Color { get; set; }
        public ColorEventArgs(ReaderColorConfig color)
        {
            Color = color;
        }
    }
}
