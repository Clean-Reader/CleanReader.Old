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
using Yuenov.SDK.Models.Discovery;
using Yuenov.SDK.Models.Share;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Layout
{
    public sealed partial class HorizonBookListView : UserControl
    {
        public event RoutedEventHandler AllButtonClick;
        public event EventHandler<Book> ItemClick;
        public HorizonBookListView()
        {
            this.InitializeComponent();
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CheckButtonStatus();
        }

        public BookListContainerBase ItemsSource
        {
            get { return (BookListContainerBase)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(BookListContainerBase), typeof(HorizonBookListView), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSource_Changed)));

        private static void ItemsSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is BookListContainerBase data)
            {
                var list = data.BookList;
                var first = list.First();
                var instance = d as HorizonBookListView;
                instance.FirstCard.Data = first;
                instance.OtherListView.ItemsSource = list.Skip(1).ToList();
                instance.CheckButtonStatus();
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(HorizonBookListView), new PropertyMetadata(""));



        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            AllButtonClick?.Invoke(sender, e);
        }

        private void OtherListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Book;
            ItemClick?.Invoke(this, book);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            CheckButtonStatus();
        }

        private void CheckButtonStatus()
        {
            var viewer = ScrollViewer;
            double horizonOffset = viewer.HorizontalOffset;
            double scrollWidth = viewer.ScrollableWidth;
            if (scrollWidth <= 0)
            {
                LeftButton.Visibility = Visibility.Collapsed;
                RightButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (horizonOffset <= 0)
                    LeftButton.Visibility = Visibility.Collapsed;
                else
                    LeftButton.Visibility = Visibility.Visible;
                if (horizonOffset + viewer.ViewportWidth >= viewer.ExtentWidth)
                    RightButton.Visibility = Visibility.Collapsed;
                else
                    RightButton.Visibility = Visibility.Visible;
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            double offset = ScrollViewer.HorizontalOffset - ScrollViewer.ViewportWidth;
            if (offset < 0)
                offset = 0;
            ScrollViewer.ChangeView(offset, 0, 1);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            double offset = ScrollViewer.HorizontalOffset + ScrollViewer.ViewportWidth;
            ScrollViewer.ChangeView(offset, 0, 1);
        }

        private void ExtraButton_Click(object sender, RoutedEventArgs e)
        {
            var first = ItemsSource.BookList.First();
            ItemClick?.Invoke(this, first);
        }
    }
}
