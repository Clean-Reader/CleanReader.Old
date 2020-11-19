using Lib.Share.Enums;
using Microsoft.Toolkit.Uwp.Helpers;
using Richasy.Helper.UWP;
using Richasy.Helper.UWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
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
    public sealed partial class iCiBaTranslateBlock : UserControl
    {
        private MediaPlayer _player = null;
        private MediaSource _source = null;
        private iCiBaHelper _helper = new iCiBaHelper(App.Tools);
        public iCiBaTranslateBlock()
        {
            this.InitializeComponent();
        }
        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register("SelectedText", typeof(string), typeof(iCiBaTranslateBlock), new PropertyMetadata(null, new PropertyChangedCallback(SelectedText_Changed)));

        private async static void SelectedText_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is string text)
            {
                var instance = d as iCiBaTranslateBlock;
                await instance.Init(text);
            }
        }

        private async Task Init(string SelectedText)
        {
            if (string.IsNullOrEmpty(SelectedText))
            {
                NoDataBlock.Visibility = Visibility.Visible;
                DetailContainer.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                NoDataBlock.Visibility = Visibility.Collapsed;
                DetailContainer.Visibility = Visibility.Visible;
            }
            ExplainBlock.Text = "";
            PhoneticBlock.Text = "--";
            SearchTextBlock.Text = SelectedText;
            bool isOk = false;
            EnglishPhoneticContainer.Visibility = Visibility.Visible;
            TranslateLoadingBar.Visibility = Visibility.Visible;
            string lan = App.Tools.App.GetLocalSetting(SettingNames.Language, "zh_CN").Substring(0, 2);
            var data = await _helper.GetWebICiBaModel(SelectedText, lan);
            if (data != null && !string.IsNullOrEmpty(data.word_name) && data.symbols != null && data.symbols.Length > 0)
            {
                InitPhoneticBlock(data);
                InitSoundButton(data);
                if (!string.IsNullOrEmpty(SoundButton.Tag.ToString()))
                    SoundButton.Visibility = Visibility.Visible;
                else
                    SoundButton.Visibility = Visibility.Collapsed;
                var builder = new StringBuilder();
                foreach (var item in data.symbols.First().parts)
                {
                    builder.AppendLine(item.part);
                    if (item.means != null && item.means.Length > 0)
                        builder.AppendLine(string.Join("; ", item.means));
                    else
                        builder.AppendLine(string.Join("; ", item.means_other.Select(p => p.word_mean)));
                }
                isOk = true;
                ExplainBlock.Text = builder.ToString().Trim();
            }
            TranslateLoadingBar.Visibility = Visibility.Collapsed;
            if (!isOk)
            {
                ExplainBlock.Text = "";
                SoundButton.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(ExplainBlock.Text))
            {
                await ExplainTranslate(SelectedText);
            }
        }
        private void InitPhoneticBlock(Ciba data)
        {
            var symbol = data.symbols.First();
            string text = "--";
            if (!string.IsNullOrEmpty(symbol.ph_am))
                text = symbol.ph_am;
            else if (!string.IsNullOrEmpty(symbol.ph_en))
                text = symbol.ph_en;
            else if (!string.IsNullOrEmpty(symbol.ph_other))
                text = symbol.ph_other;
            else if (!string.IsNullOrEmpty(symbol.word_symbol))
                text = symbol.word_symbol;
            text = text.Replace("http://res-tts.iciba.com", "");
            PhoneticBlock.Text = text;
        }
        private void InitSoundButton(Ciba data)
        {
            var symbol = data.symbols.First();
            if (!string.IsNullOrEmpty(symbol.ph_am_mp3))
                SoundButton.Tag = symbol.ph_am_mp3;
            else if (!string.IsNullOrEmpty(symbol.ph_en_mp3))
                SoundButton.Tag = symbol.ph_en_mp3;
            else if (!string.IsNullOrEmpty(symbol.ph_tts_mp3))
                SoundButton.Tag = symbol.ph_tts_mp3;
            else if (!string.IsNullOrEmpty(symbol.symbol_mp3))
                SoundButton.Tag = symbol.symbol_mp3;
            else
                SoundButton.Tag = "";
        }
        private async Task ExplainTranslate(string SelectedText)
        {
            EnglishPhoneticContainer.Visibility = Visibility.Collapsed;
            TranslateLoadingBar.Visibility = Visibility.Visible;
            string lan = App.Tools.App.GetLocalSetting(SettingNames.Language, "zh_CN").Substring(0, 2);
            var data = await _helper.GetICiBaTranslate(SelectedText, lan);
            if (data != null)
            {
                ExplainBlock.Text = data.content.@out ?? "";
            }
            TranslateLoadingBar.Visibility = Visibility.Collapsed;
        }
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            string url = "";
            string lan = IsHasCHZN(SelectedText) ? "zh" : "en";
            string text = WebUtility.UrlEncode(SelectedText);
            switch (btn.Tag.ToString())
            {
                case "Baidu":
                    url = $"https://baike.baidu.com/item/{SelectedText}";
                    break;
                case "Wiki":
                    url = $"https://{lan}.wikipedia.org/wiki/{SelectedText}";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(url))
            {
                await Launcher.LaunchUriAsync(new Uri(url));
            }
        }
        /// <summary>
        /// 检测文本是否包含中文
        /// </summary>
        /// <param name="inputData">文本</param>
        /// <returns></returns>
        public bool IsHasCHZN(string inputData)
        {
            Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }

        private void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender) as Button;
            string uri = btn.Tag.ToString();
            if (!string.IsNullOrEmpty(uri))
            {
                if (_player == null)
                {
                    _player = new MediaPlayer();
                    _player.MediaEnded += Media_Ended;
                }
                _source = MediaSource.CreateFromUri(new Uri(uri));
                _player.Source = _source;
                SoundButton.IsLoading = true;
                _player.Play();
            }

        }

        private async void Media_Ended(MediaPlayer sender, object args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                SoundButton.IsLoading = false;
                if (_source != null)
                {
                    _source.Dispose();
                    _source = null;
                }
                _player.Source = null;
            });
        }
    }
}
