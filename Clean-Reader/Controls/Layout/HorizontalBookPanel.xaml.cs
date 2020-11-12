using Lib.Share.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using yuenov = Yuenov.SDK.Models.Share;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Layout
{
    public sealed partial class HorizontalBookPanel : UserControl
    {
        private ObservableCollection<Book> BookCollection = new ObservableCollection<Book>();
        public HorizontalBookPanel()
        {
            this.InitializeComponent();
        }
        public event RoutedEventHandler AllButtonClick;
        public event EventHandler<Book> ItemClick;

        public List<yuenov.Book> BookList
        {
            get { return (List < yuenov.Book>)GetValue(BookListProperty); }
            set { SetValue(BookListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BookList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookListProperty =
            DependencyProperty.Register("BookList", typeof(List<yuenov.Book>), typeof(HorizontalBookPanel), new PropertyMetadata(null,new PropertyChangedCallback(BookList_Changed)));

        private static void BookList_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue !=null && e.NewValue is List<yuenov.Book> books)
            {
                var temp = books.Select(p => new Book(p));
                var instance = d as HorizontalBookPanel;
                instance.BookCollection.Clear();
                temp.ToList().ForEach(p => instance.BookCollection.Add(p));
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(HorizontalBookPanel), new PropertyMetadata(""));



        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            AllButtonClick?.Invoke(this, e);
        }

        private void BookListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClick?.Invoke(this, e.ClickedItem as Book);
        }
    }
}
