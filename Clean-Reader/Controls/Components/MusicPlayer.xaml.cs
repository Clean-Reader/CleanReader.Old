using Lib.Share.Enums;
using Lib.Share.Models;
using System;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Clean_Reader.Controls.Components
{
    public sealed partial class MusicPlayer : UserControl
    {
        public event EventHandler MediaEnded;
        private bool _isLoaded = false;

        public bool IsMediaEnded = false;
        public MusicPlayer()
        {
            this.InitializeComponent();
            App.VM._musicPlayer = this;
        }
        public void Check()
        {
            if (Book != App.VM.CurrentBook && Book != null)
            {
                Close();
            }
        }

        public void Close()
        {
            if (App.VM._player == null)
                return;
            App.VM._player.Pause();
            IsMediaEnded = false;
            try
            {
                var item = App.VM._player.Source as MediaPlaybackItem;
                item.Source.Dispose();
            }
            catch (Exception)
            { }
            this.Visibility = Visibility.Collapsed;
        }

        public Book Book
        {
            get { return (Book)GetValue(BookProperty); }
            set { SetValue(BookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Book.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookProperty =
            DependencyProperty.Register("Book", typeof(Book), typeof(MusicPlayer), new PropertyMetadata(null));



        public MediaPlaybackItem Source
        {
            get { return (MediaPlaybackItem)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(MediaPlaybackItem), typeof(MusicPlayer), new PropertyMetadata(null, new PropertyChangedCallback(Source_Changed)));

        private static void Source_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is MediaPlaybackItem data)
            {
                var instance = d as MusicPlayer;
                if (App.VM._player == null)
                {
                    App.VM._player = new MediaPlayer();
                    App.VM._player.MediaEnded += instance.MediaPlayer_MediaEnded;
                    instance.MPE.SetMediaPlayer(App.VM._player);
                }
                instance.IsMediaEnded = false;
                App.VM._player.Source = data;
                instance.Book = App.VM.CurrentBook;
                if (instance._isLoaded)
                {
                    instance.TransportControls.Book = App.VM.CurrentBook;
                    instance.TransportControls.ChapterName = App.VM._reader.CurrentChapter?.Title ?? "--";
                }
            }
        }

        private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                IsMediaEnded = true;
                MediaEnded?.Invoke(this, EventArgs.Empty);
            });
        }

        private async void TransportControls_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var stream = App.VM._reader.GetCurrentSpeechStream();
            if (stream != null)
            {
                string fileName = App.VM.CurrentBook.Name + " - " + App.VM._reader.CurrentChapter.Title;
                var file = await App.Tools.IO.GetSaveFileAsync(".wav", fileName + ".wav", "WAV File");
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    IBuffer buffer = reader.ReadBuffer((uint)stream.Size);
                    await FileIO.WriteBufferAsync(file, buffer);
                }
                App.VM.ShowPopup(LanguageNames.SaveSuccess);
            }
        }

        private void TransportControls_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                TransportControls.Book = App.VM.CurrentBook;
                TransportControls.ChapterName = App.VM._reader.CurrentChapter?.Title ?? "--";
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 30)
                TransportControls.MaxWidth = e.NewSize.Width - 30;
        }
    }
}
