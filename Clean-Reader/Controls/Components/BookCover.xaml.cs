using Lib.Share.Enums;
using Lib.Share.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class BookCover : UserControl
    {
        public BookCover()
        {
            this.InitializeComponent();
            string theme = App.Current.RequestedTheme.ToString();
            CoverImage.PlaceholderSource = new BitmapImage(new System.Uri($"ms-appx:///Assets/coverholder-{theme}.png")) { DecodePixelWidth = 120 };
            BackgroundImage.PlaceholderSource = new BitmapImage(new System.Uri($"ms-appx:///Assets/coverholder-{theme}.png")) { DecodePixelWidth = 40 };
        }

        public Book Data
        {
            get { return (Book)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Book), typeof(BookCover), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is Book data)
            {
                var instance = d as BookCover;
                switch (data.Type)
                {
                    case BookType.Txt:
                        instance.CoverImageContainer.Visibility = Visibility.Collapsed;
                        instance.SimpleCoverContainer.Visibility = Visibility.Visible;
                        instance.BookNameBlock.Text = data.Name;
                        instance.SimpleCover.Background = instance.BackgroundRect.Background = App.Tools.App.GetThemeBrushFromResource(ColorNames.TxtColor);
                        break;
                    case BookType.Epub:
                        instance.CoverImageContainer.Visibility = Visibility.Visible;
                        instance.SimpleCoverContainer.Visibility = Visibility.Collapsed;
                        instance.CoverImage.Source = new BitmapImage(new System.Uri(data.Cover)) { DecodePixelWidth = 140 };
                        if (instance.BackgroundImage.Visibility == Visibility.Visible)
                            instance.BackgroundImage.Source = new BitmapImage(new System.Uri(data.Cover)) { DecodePixelWidth = 40 };
                        break;
                    case BookType.Web:
                        instance.CoverImageContainer.Visibility = Visibility.Visible;
                        instance.SimpleCoverContainer.Visibility = Visibility.Collapsed;
                        instance.CoverImage.Source = new BitmapImage(new System.Uri(App.VM._yuenovClient.GetImageUrl(data.Cover))) { DecodePixelWidth = 140 };
                        if (instance.BackgroundImage.Visibility == Visibility.Visible)
                            instance.BackgroundImage.Source = new BitmapImage(new System.Uri(App.VM._yuenovClient.GetImageUrl(data.Cover))) { DecodePixelWidth = 40 };
                        break;
                    default:
                        break;
                }
            }
        }

        private void CoverImage_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            CoverImageContainer.Visibility = Visibility.Collapsed;
            SimpleCoverContainer.Visibility = Visibility.Visible;
            BookNameBlock.Text = Data.Name;
            if (Data.Type == BookType.Epub)
                SimpleCover.Background = BackgroundRect.Background = App.Tools.App.GetThemeBrushFromResource(ColorNames.EpubColor);
            else
                SimpleCover.Background = BackgroundRect.Background = App.Tools.App.GetThemeBrushFromResource(ColorNames.TxtColor);
        }
    }
}
