using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Lib.Share.Models;
using Richasy.Controls.UWP.Models.UI;
using Richasy.Font.UWP;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        public SettingPage() : base()
        {
            IsInit = false;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Init();
            base.OnNavigatedTo(e);
        }

        private async void Init()
        {
            IsInit = false;
            string theme = App.Tools.App.GetLocalSetting(SettingNames.Theme, StaticString.ThemeSystem);
            switch (theme)
            {
                case StaticString.ThemeSystem:
                    ThemeComboBox.SelectedIndex = 0;
                    break;
                case StaticString.ThemeLight:
                    ThemeComboBox.SelectedIndex = 1;
                    break;
                case StaticString.ThemeDark:
                    ThemeComboBox.SelectedIndex = 2;
                    break;
                default:
                    break;
            }
            string language = App.Tools.App.GetLocalSetting(SettingNames.Language, StaticString.LanZh);
            switch (language)
            {
                case StaticString.LanZh:
                    LanguageComboBox.SelectedIndex = 0;
                    break;
                case StaticString.LanEn:
                    LanguageComboBox.SelectedIndex = 1;
                    break;
                default:
                    break;
            }
            string appFont = App.Tools.App.GetLocalSetting(SettingNames.FontFamily, StaticString.FontDefault);
            if (vm.FontCollection.Count == 0)
            {
                var fonts = SystemFont.GetSystemFonts().OrderBy(p => p.Name).ToList();
                fonts.ForEach(p => vm.FontCollection.Add(p));
            }
            FontComboBox.SelectedItem = vm.FontCollection.Where(p => p.Name == vm.ReaderStyle.FontFamily).FirstOrDefault();
            double fontSize = Convert.ToDouble(App.Tools.App.GetLocalSetting(SettingNames.FontSize, "14"));
            FontSizeBox.Value = fontSize;
            string searchEngine = App.Tools.App.GetLocalSetting(SettingNames.SearchEngine, StaticString.SearchBing);
            switch (searchEngine)
            {
                case StaticString.SearchGoogle:
                    SearchEngineComboBox.SelectedIndex = 0;
                    break;
                case StaticString.SearchBing:
                    SearchEngineComboBox.SelectedIndex = 1;
                    break;
                case StaticString.SearchBaidu:
                    SearchEngineComboBox.SelectedIndex = 2;
                    break;
                default:
                    break;
            }
            await Task.Delay(100);
            IsInit = true;
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInit)
            {
                var item = LanguageComboBox.SelectedItem as ComboBoxItem;
                App.Tools.App.WriteLocalSetting(SettingNames.Language, item.Tag.ToString());
                vm.ShowRestartDialog();
            }
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInit)
            {
                var item = ThemeComboBox.SelectedItem as ComboBoxItem;
                App.Tools.App.WriteLocalSetting(SettingNames.Theme, item.Tag.ToString());
                vm.ShowRestartDialog();
            }
        }

        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInit)
            {
                var item = FontComboBox.SelectedItem as SystemFont;
                App.Tools.App.WriteLocalSetting(SettingNames.FontFamily, item.Name);
                vm.ShowRestartDialog();
            }
        }

        private void FontSizeBox_ValueChanged(object sender, double e)
        {
            if (IsInit)
            {
                App.Tools.App.WriteLocalSetting(SettingNames.FontSize, e.ToString());
            }
        }

        private void SearchEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInit)
            {
                var item = SearchEngineComboBox.SelectedItem as ComboBoxItem;
                App.Tools.App.WriteLocalSetting(SettingNames.SearchEngine, item.Tag.ToString());
            }
        }
    }
}
