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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Yuenov.SDK.Models.Share;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class YuenovBookCard : UserControl
    {
        public event RoutedEventHandler TagButtonClick;
        public YuenovBookCard()
        {
            this.InitializeComponent();
        }

        public Book Data
        {
            get { return (Book)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Book), typeof(YuenovBookCard), new PropertyMetadata(null,new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null && e.NewValue is Book book)
            {
                var instance = d as YuenovBookCard;
                instance.CoverImage.Source = new BitmapImage(new Uri(App.VM._yuenovClient.GetImageUrl(book.CoverImg))) { DecodePixelWidth = 120 };
                instance.BackgroundImage.Source = new BitmapImage(new Uri(App.VM._yuenovClient.GetImageUrl(book.CoverImg))) { DecodePixelWidth = 40 };
                instance.NameBlock.Text = book.Title;
                ToolTipService.SetToolTip(instance.NameBlock, book.Title);
                instance.AuthorBlock.Text = book.Author;
                instance.PropertyBlock.Text = $"{book.CategoryName} · {book.Word}";
                instance.DescriptionBlock.Text = book.Description;
                ToolTipService.SetToolTip(instance.DescriptionBlock, book.Description);
                instance.StatusBlock.Text = book.ChapterStatus == Yuenov.SDK.Enums.ChapterStatus.Serialize ? "连载中" : "已完结";
            }
        }

        public Visibility TagButtonVisibility
        {
            get { return (Visibility)GetValue(TagButtonVisibilityProperty); }
            set { SetValue(TagButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagButtonVisibilityProperty =
            DependencyProperty.Register("TagButtonVisibility", typeof(Visibility), typeof(YuenovBookCard), new PropertyMetadata(Visibility.Visible));



        private void TagButton_Click(object sender, RoutedEventArgs e)
        {
            TagButtonClick?.Invoke(sender, e);
        }
    }
}
