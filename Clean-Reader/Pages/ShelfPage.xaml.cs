using Clean_Reader.Models.Core;
using Lib.Share.Models;
using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShelfPage : RichasyPage
    {
        public AppViewModel vm = App.VM;
        public ShelfPage()
        {
            this.InitializeComponent();
            vm.LastestReadCollection.CollectionChanged += LastestReadCollection_Changed;
            vm.DisplayBookCollection.CollectionChanged += DisplayBookCollection_Changed;
            LastestContainer.Visibility = vm.LastestReadCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            NoDataContainer.Visibility = vm.DisplayBookCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DisplayBookCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoDataContainer.Visibility = vm.DisplayBookCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LastestReadCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            LastestContainer.Visibility = vm.LastestReadCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ImportButton.IsEnabled = false;
            await vm.ImportBooks();
            ImportButton.IsEnabled = true;
        }

        private void BookView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Book;
            vm.OpenReaderView(book);
        }
    }
}
