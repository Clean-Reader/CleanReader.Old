using Clean_Reader.Models.Core;
using Clean_Reader.Models.UI;
using Lib.Share.Enums;
using Lib.Share.Models;
using Microsoft.Toolkit.Uwp.Helpers;
using Richasy.Controls.UWP.Models.UI;
using Richasy.Font.UWP;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
        public ObservableCollection<string> BackgroundCollection = new ObservableCollection<string>();
        public SettingPage() : base()
        {
            IsInit = false;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Init();
            Focus(FocusState.Programmatic);
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
            FontComboBox.SelectedItem = vm.FontCollection.Where(p => p.Name == appFont).FirstOrDefault();
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
            bool isEnableImage = App.Tools.App.GetBoolSetting(SettingNames.EnabledBackgroundImage, false);
            EnableBackgroundImageToggleSwitch.IsOn = isEnableImage;
            var color = App.Tools.App.GetLocalSetting(SettingNames.BackgroundMaskColor, App.Current.RequestedTheme == ApplicationTheme.Light ? "#C7FFFFFF" : "#C7000000").Hex16toRGB();
            MaskColorPicker.Color = color;
            bool isAutoOpenLastBook = App.Tools.App.GetBoolSetting(SettingNames.IsAutoOpenLastBook, false);
            AutoOpenLastBookSwitch.IsOn = isAutoOpenLastBook;
            bool isAutoCheckUpdate = App.Tools.App.GetBoolSetting(SettingNames.IsEnableAutoCheckUpdate, false);
            AutoCheckWebBookSwitch.IsOn = isAutoCheckUpdate;
            int lastUpdateSec = Convert.ToInt32(App.Tools.App.GetLocalSetting(SettingNames.LastBackgroundSyncTime, "0"));
            var date = DateTimeOffset.FromUnixTimeSeconds(lastUpdateSec);
            string time = lastUpdateSec == 0 ? "--" : date.ToString("yyyy/MM/dd HH:mm");
            LastUpdateTimeBlock.Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.LastUpdateTime) + time;
            bool isDisableScale = App.Tools.App.GetBoolSetting(SettingNames.DisableXboxScale,false);
            DisableXboxScaleSwitch.IsOn = isDisableScale;
            DisableXboxScaleSwitch.IsEnabled = SystemInformation.DeviceFamily == "Windows.Xbox";
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

        private async void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            AddImageButton.IsLoading = true;
            var image = await App.Tools.IO.OpenLocalFileAsync(".png", ".jpg", ".bmp", ".jpeg", ".gif");
            await vm.AddBackgroundImage(image);
            AddImageButton.IsLoading = false;
        }

        private void ImageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (IsInit)
            {
                var source = e.ClickedItem as string;
                App.Tools.App.WriteLocalSetting(SettingNames.BackgroundImage, source);
                vm.BackgroundImageToggle();
            }
        }

        private async void OnElementClicked(object sender, RoutedEventArgs e)
        {
            string source = (sender as FrameworkElement).DataContext as string;
            if ((sender as AppBarButton).Tag.ToString() == "Zoom")
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(source));
                await Launcher.LaunchFileAsync(file);
            }
            else
            {
                await vm.RemoveBackgroundImage(source);
            }
        }

        private void EnableBackgroundImageToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (IsInit)
            {
                App.Tools.App.WriteLocalSetting(SettingNames.EnabledBackgroundImage, EnableBackgroundImageToggleSwitch.IsOn.ToString());
                vm.BackgroundImageToggle();
            }
        }

        private void MaskColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (!IsInit)
                return;
            MaskDisplay.Background = new SolidColorBrush(args.NewColor);
            App.Tools.App.WriteLocalSetting(SettingNames.BackgroundMaskColor, args.NewColor.ToString());
            vm.BackgroundImageToggle();
        }

        private void AutoOpenLastBookSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsInit)
                return;
            App.Tools.App.WriteLocalSetting(SettingNames.IsAutoOpenLastBook, AutoOpenLastBookSwitch.IsOn.ToString());
        }

        private async void AutoCheckWebBookSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsInit)
                return;
            App.Tools.App.WriteLocalSetting(SettingNames.IsEnableAutoCheckUpdate, AutoCheckWebBookSwitch.IsOn.ToString());
            if (AutoCheckWebBookSwitch.IsOn)
                await vm.RegisterBackgroundTask(StaticString.TaskAutoCheck);
            else
                vm.UnRegisterBackgroundTask(StaticString.TaskAutoCheck);
        }

        private void DisableXboxScaleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (IsInit)
            {
                App.Tools.App.WriteLocalSetting(SettingNames.DisableXboxScale, DisableXboxScaleSwitch.IsOn.ToString());
                vm.ShowRestartDialog();
            }
        }
    }
}
