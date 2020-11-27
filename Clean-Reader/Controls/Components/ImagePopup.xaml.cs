using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Clean_Reader.Controls.Components
{
    public sealed partial class ImagePopup : UserControl
    {
        Guid id = Guid.NewGuid();
        private Popup _popup = null;
        private IRandomAccessStream stream;
        private string _base64;
        public ImagePopup()
        {
            this.InitializeComponent();
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            _popup = new Popup();
            _popup.Child = this;

        }

        public async Task SetSource(string base64)
        {
            _base64 = base64;
            var byteArray = Convert.FromBase64String(base64);
            stream = byteArray.AsBuffer().AsStream().AsRandomAccessStream();
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            DisplayImage.Source = bitmap;
        }

        public void Show()
        {
            _popup.IsOpen = true;
            PopupContainer.Visibility = Visibility.Visible;
            App.Tools.AddWindowSizeChangeAction(id, (size) =>
            {
                Width = size.Width;
                Height = size.Height;
            });
        }

        public void Hide()
        {
            PopupContainer.Visibility = Visibility.Collapsed;
            stream.Dispose();
            App.Tools.RemoveWindowSizeChangeAction(id);
            _popup.IsOpen = true;
        }

        private void DisplayImage_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Hide();
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Grid)
            {
                Hide();
            }
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            stream.Seek(0);
            package.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            Clipboard.SetContent(package);
            App.VM.ShowPopup(Lib.Share.Enums.LanguageNames.Copied);
        }

        private async void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            var file = await App.Tools.IO.GetSaveFileAsync(".png", "Image.png", "PNG File");
            if (file != null)
            {
                var bt = Convert.FromBase64String(_base64);
                var image = bt.AsBuffer();
                await FileIO.WriteBufferAsync(file, image);
                App.VM.ShowPopup(Lib.Share.Enums.LanguageNames.SaveSuccess);
            }
        }
    }
}
