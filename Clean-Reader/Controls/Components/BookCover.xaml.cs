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
                        instance.CoverImage.Visibility = Visibility.Collapsed;
                        instance.SimpleCover.Visibility = Visibility.Visible;
                        instance.BookNameBlock.Text = data.Name;
                        instance.SimpleCover.Background = instance.BackgroundRect.Background = App.Tools.App.GetThemeBrushFromResource(ColorNames.TxtColor);
                        break;
                    case BookType.Epub:
                        instance.CoverImage.Visibility = Visibility.Visible;
                        instance.SimpleCover.Visibility = Visibility.Collapsed;
                        instance.CoverImage.Source = new BitmapImage(new System.Uri(data.Cover)) { DecodePixelWidth = 140 };
                        if (instance.BackgroundImage.Visibility == Visibility.Visible)
                            instance.BackgroundImage.Source = new BitmapImage(new System.Uri(data.Cover)) { DecodePixelWidth = 40 };
                        break;
                    case BookType.Web:
                        instance.CoverImage.Visibility = Visibility.Visible;
                        instance.SimpleCover.Visibility = Visibility.Collapsed;
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
            CoverImage.Visibility = Visibility.Collapsed;
            SimpleCover.Visibility = Visibility.Visible;
            BookNameBlock.Text = Data.Name;
            if (Data.Type == BookType.Epub)
                SimpleCover.Background = BackgroundRect.Background = App.Tools.App.GetThemeBrushFromResource(ColorNames.EpubColor);
            else
                SimpleCover.Background = BackgroundRect.Background = App.Tools.App.GetThemeBrushFromResource(ColorNames.TxtColor);
        }
    }
}
