using Lib.Share.Enums;
using Richasy.Controls.Reader.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class OtherOptionPanel : UserControl
    {
        public event EventHandler<Regex> CustomRegexSubmit;
        public event EventHandler<double> MaxSingleColumnChanged;
        private bool _isInit = false;
        public OtherOptionPanel()
        {
            this.InitializeComponent();
        }
        public void Init()
        {
            _isInit = false;
            double maxWidth = Convert.ToDouble(App.Tools.App.GetLocalSetting(SettingNames.MaxSingleColumnWidth, "900"));
            MaxSingleColumnWidthSlider.Value = maxWidth;
            if (App.VM._reader.ReaderType != ReaderType.Txt)
            {
                RegexBox.IsEnabled = false;
                RegexTestButton.IsEnabled = false;
            }
            else
            {
                RegexBox.IsEnabled = true;
                RegexTestButton.IsEnabled = true;
            }
            RegexBox.Text = App.VM.CurrentBook.CustomRegex ?? "";
            VoiceComboBox.ItemsSource = SpeechSynthesizer.AllVoices.ToList();
            string voiceId = App.Tools.App.GetLocalSetting(SettingNames.SpeechVoice, SpeechSynthesizer.DefaultVoice.Id);
            for (int i = 0; i < SpeechSynthesizer.AllVoices.Count; i++)
            {
                if (SpeechSynthesizer.AllVoices[i].Id == voiceId)
                {
                    VoiceComboBox.SelectedIndex = i;
                    break;
                }
            }
            _isInit = true;
        }
        private void RegexTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(RegexBox.Text))
                {
                    var regex = new Regex(RegexBox.Text);
                    CustomRegexSubmit?.Invoke(sender, regex);
                }
                else
                    CustomRegexSubmit?.Invoke(sender, null);
            }
            catch (Exception ex)
            {
                App.VM.ShowPopup(ex.Message, true);
            }
        }

        private void MaxSingleColumnWidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_isInit)
                return;
            MaxSingleColumnChanged?.Invoke(this, e.NewValue);
        }

        private void VoiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            App.Tools.App.WriteLocalSetting(SettingNames.SpeechVoice, (VoiceComboBox.SelectedItem as VoiceInformation).Id);
        }
    }
}
