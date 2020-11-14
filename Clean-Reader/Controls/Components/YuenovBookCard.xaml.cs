using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Yuenov.SDK.Models.Share;
using Yuenov.SDK.Models.Store;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class YuenovBookCard : UserControl
    {
        public YuenovBookCard()
        {
            this.InitializeComponent();
            string theme = App.Current.RequestedTheme.ToString();
            CoverImage.PlaceholderSource = new BitmapImage(new System.Uri($"ms-appx:///Assets/coverholder-{theme}.png")) { DecodePixelWidth = 120 };
        }

        public Thickness InnerPadding
        {
            get { return (Thickness)GetValue(InnerPaddingProperty); }
            set { SetValue(InnerPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerPaddingProperty =
            DependencyProperty.Register("InnerPadding", typeof(Thickness), typeof(YuenovBookCard), new PropertyMetadata(new Thickness(25)));



        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(YuenovBookCard), new PropertyMetadata(380d));

        public new double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(YuenovBookCard), new PropertyMetadata(185d));





        public Book Data
        {
            get { return (Book)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Book), typeof(YuenovBookCard), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is Book book)
            {
                var instance = d as YuenovBookCard;
                instance.CoverImageContainer.Visibility = Visibility.Visible;
                instance.SimpleCoverContainer.Visibility = Visibility.Collapsed;
                instance.CoverImage.Source = new BitmapImage(new Uri(App.VM._yuenovClient.GetImageUrl(book.CoverImg))) { DecodePixelWidth = 120 };
                instance.NameBlock.Text = book.Title;
                ToolTipService.SetToolTip(instance.NameBlock, book.Title);
                instance.AuthorBlock.Text = book.Author;
                instance.PropertyBlock.Text = $"{book.CategoryName} · {book.Word}";
                if (book is BookDetail detail)
                {
                    instance.DetailChapterContainer.Visibility = Visibility.Visible;
                    instance.DescriptionBlock.Visibility = Visibility.Collapsed;
                    instance.ChapterNumberBlock.Text = $"共{detail.ChapterNumber}章";
                    if (detail.Update != null)
                    {
                        instance.LastestChapterBlock.Text = $"最新章节：{detail.Update.ChapterName}";
                        instance.StatusBlock.Text = detail.Update.ChapterStatus == Yuenov.SDK.Enums.ChapterStatus.Serialize ? "连载中" : "已完结";
                    }
                }
                else
                {
                    instance.DetailChapterContainer.Visibility = Visibility.Collapsed;
                    instance.DescriptionBlock.Visibility = Visibility.Visible;
                    instance.DescriptionBlock.Text = book.Description;
                    instance.StatusBlock.Text = book.ChapterStatus == Yuenov.SDK.Enums.ChapterStatus.Serialize ? "连载中" : "已完结";
                }

                ToolTipService.SetToolTip(instance.DescriptionBlock, book.Description);
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



        private void CoverImage_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            BookNameBlock.Text = Data.Title;
            CoverImageContainer.Visibility = Visibility.Collapsed;
            SimpleCoverContainer.Visibility = Visibility.Visible;
        }
    }
}
