using Richasy.Controls.Reader.Models;
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

namespace Clean_Reader.Controls.Layout
{
    public sealed partial class InsideSearchPanel : UserControl
    {
        public event EventHandler<string> QuerySubmit;
        public event EventHandler<InsideSearchItem> ItemClick;
        public InsideSearchPanel()
        {
            this.InitializeComponent();
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(InsideSearchPanel), new PropertyMetadata(null));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(InsideSearchPanel), new PropertyMetadata(false));

        public Visibility NoDataVisibility
        {
            get { return (Visibility)GetValue(NoDataVisibilityProperty); }
            set { SetValue(NoDataVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoDataVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoDataVisibilityProperty =
            DependencyProperty.Register("NoDataVisibility", typeof(Visibility), typeof(InsideSearchPanel), new PropertyMetadata(Visibility.Collapsed));

        public void Reset()
        {
            KeywordSearchBox.Text = "";
            NoDataVisibility = Visibility.Collapsed;
            IsLoading = false;
        }

        public void Init(string text)
        {
            KeywordSearchBox.Text = text;
            QuerySubmit?.Invoke(this, text);
        }

        private void KeywordSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmit?.Invoke(this, args.QueryText);
        }

        private void ResultListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClick?.Invoke(this, e.ClickedItem as InsideSearchItem);
        }
    }
}
