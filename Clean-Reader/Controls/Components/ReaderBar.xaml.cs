using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Richasy.Controls.UWP.Interaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class ReaderBar : UserControl
    {
        AppViewModel vm = App.VM;
        public event RoutedEventHandler BackButtonClick;
        public event RoutedEventHandler ChapterButtonClick;
        public event RoutedEventHandler SearchButtonClick;
        public bool IsShow
        {
            get => MenuContainer.Visibility == Visibility.Visible;
        }
        public ReaderBar()
        {
            this.InitializeComponent();
            App.VM._readerBar = this;
        }
        public bool IsContainPlayer
        {
            get => MenuContainer.Children.Contains(App.VM._musicPlayer);
        }
        public void Init()
        {
            ColorConfigPanel.Init();
            FontPanel.Init();
            OtherConfigPanel.Init();
        }
        public void Show()
        {
            MenuContainer.Visibility = Visibility.Visible;
        }
        public void Hide()
        {
            MenuContainer.Visibility = Visibility.Collapsed;
        }
        public void Toggle()
        {
            MenuContainer.Visibility = MenuContainer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        private void ChapterButton_Click(object sender, RoutedEventArgs e)
        {
            ChapterButtonClick?.Invoke(this, e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MenuContainer.Visibility = Visibility.Collapsed;
            BackButtonClick?.Invoke(this, e);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MenuContainer.Visibility = Visibility.Collapsed;
            SearchButtonClick?.Invoke(this, e);
        }

        private void OtherConfigPanel_MaxSingleColumnChanged(object sender, double e)
        {
            App.VM._reader.SingleColumnMaxWidth = e;
            App.Tools.App.WriteLocalSetting(SettingNames.MaxSingleColumnWidth, e.ToString());
        }

        private async void OtherConfigPanel_CustomRegexSubmit(object sender, Regex e)
        {
            var btn = sender as ActionButton;
            btn.IsLoading = true;
            await App.VM.RebuildChapter(e);
            btn.IsLoading = false;
            App.VM.ShowPopup(LanguageNames.RebuildSuccess);
        }

        private void OtherFlyout_Opened(object sender, object e)
        {
            OtherConfigPanel.Init();
        }

        private async void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadSpeech();
        }

        public async Task LoadSpeech()
        {
            SpeechButton.IsLoading = true;
            var reader = App.VM._reader;
            var syn = new SpeechSynthesizer();
            string id = App.Tools.App.GetLocalSetting(SettingNames.SpeechVoice, SpeechSynthesizer.DefaultVoice.Id);
            var voice = SpeechSynthesizer.AllVoices.Where(p => p.Id == id).FirstOrDefault();
            if (voice != null)
                syn.Voice = voice;
            else
                syn.Voice = SpeechSynthesizer.DefaultVoice;
            var source = await reader.GetChapterVoiceAsync(reader.CurrentChapter, true, syn);
            MusicPlayer.Source = source;
            if (MusicPlayer.Visibility == Visibility.Collapsed)
                MusicPlayer.Visibility = Visibility.Visible;
            SpeechButton.IsLoading = false;
        }

        private void MusicPlayer_MediaEnded(object sender, EventArgs e)
        {
            bool isAutoNext = App.Tools.App.GetBoolSetting(SettingNames.IsSpeechAutoNext);
            if (isAutoNext)
            {
                if(this.IsContainPlayer)
                    App.VM._reader.Next();
            }
            else
                MusicPlayer.Visibility = Visibility.Collapsed;
        }

        public void RemovePlayer()
        {
            MenuContainer.Children.Remove(App.VM._musicPlayer);
        }

        public void InsertPlayer()
        {
            MenuContainer.Children.Insert(0, App.VM._musicPlayer);
        }

        private async void BasicFlyout_Opened(object sender, object e)
        {
            (sender as Flyout).AllowFocusOnInteraction = true;
            await FocusManager.TryFocusAsync((sender as Flyout), FocusState.Programmatic);
        }
    }
}
