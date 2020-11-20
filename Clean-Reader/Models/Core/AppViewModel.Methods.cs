using Clean_Reader.Controls.Dialogs;
using Clean_Reader.Models.Enums;
using Clean_Reader.Models.UI;
using Clean_Reader.Pages;
using Lib.Share.Enums;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.Reader.Models;
using Richasy.Controls.UWP.Popups;
using Richasy.Controls.UWP.Widgets;
using Richasy.Font.UWP;
using Richasy.Font.UWP.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        /// <summary>
        /// 导航到指定页面
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="paramter">参数</param>
        public void NavigateToPage(MenuItemType type, object paramter = null)
        {
            Type pageType = null;
            switch (type)
            {
                case MenuItemType.Shelf:
                    pageType = typeof(ShelfPage);
                    break;
                case MenuItemType.Discovery:
                    pageType = typeof(DiscoveryPage);
                    break;
                case MenuItemType.CateAndRank:
                    pageType = typeof(CategoryPage);
                    break;
                case MenuItemType.Topic:
                    pageType = typeof(TopicPage);
                    break;
                case MenuItemType.Setting:
                    pageType = typeof(SettingPage);
                    break;
                default:
                    break;
            }
            if (_rootFrame.Content == null || !_rootFrame.Content.GetType().Equals(pageType))
                _rootFrame.Navigate(pageType, paramter, new DrillInNavigationTransitionInfo());
        }

        public async void OpenReaderView(Book book)
        {
            if (book == null)
                return;
            if (LastestReadCollection.Contains(book))
                LastestReadCollection.Remove(book);
            LastestReadCollection.Insert(0, book);
            int maxCount = Convert.ToInt32(App.Tools.App.GetLocalSetting(SettingNames.MaxLastestBookCount, "12"));
            if (LastestReadCollection.Count > maxCount)
                LastestReadCollection.RemoveAt(LastestReadCollection.Count - 1);

            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(ReaderPage), book, new DrillInNavigationTransitionInfo());
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileLastestList, JsonConvert.SerializeObject(LastestReadCollection.Select(p => p.BookId)));
        }

        public void CloseReaderView()
        {
            var frame = Window.Current.Content as Frame;
            if (frame.CanGoBack)
                frame.GoBack(new EntranceNavigationTransitionInfo());
        }



        private void CurrentShelf_Changed(object sender, EventArgs e)
        {
            DisplayBookCollection.Clear();
            if (TotalBookList.Count > 0)
            {
                CurrentShelfInit();
            }
        }


        public void ShowPopup(LanguageNames name, bool isError = false)
        {
            ShowPopup(App.Tools.App.GetLocalizationTextFromResource(name), isError);
        }
        public void ShowPopup(string msg, bool isError = false)
        {
            var popup = new TipPopup(App.Tools, msg);
            ColorNames color = isError ? ColorNames.ErrorColor : ColorNames.PrimaryColor;
            popup.Show(color, 2);
        }
        public void ShowWaitingPopup(LanguageNames content)
        {
            _waitPopup.Text = App.Tools.App.GetLocalizationTextFromResource(content);
            _waitPopup.Show();
        }
        public void HideWaitingPopup()
        {
            _waitPopup.Hide();
        }
        public async void CheckUpdate()
        {
            string localVersion = App.Tools.App.GetLocalSetting(SettingNames.AppVersion, "");
            if (localVersion != VersionBlock.Version)
            {
                var main = new VersionBlock();
                main.Title = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.UpdateTitle);
                string lan = App.Tools.App.GetLocalSetting(SettingNames.Language, "zh_CN");
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Misc/Update_{lan}.txt"));
                string content = await FileIO.ReadTextAsync(file);
                main.Description = content;
                main.LogoUri = "ms-appx:///Assets/logo.png";
                main.ActionButtonStyle = App.Tools.App.GetStyleFromResource(StyleNames.PrimaryActionButtonStyle);
                main.ActionIcon = new FeatherIcon(FeatherSymbol.X);
                main.TitleTextStyle = App.Tools.App.GetStyleFromResource(StyleNames.SubtitleTextStyle);
                main.DescriptionTextStyle = App.Tools.App.GetStyleFromResource(StyleNames.BasicMarkdownTextBlock);
                var popup = new CenterPopup(App.Tools);
                popup.Style = App.Tools.App.GetStyleFromResource(StyleNames.BasicCenterPopupStyle);
                popup.Main = main;
                main.ActionButtonClick += (_s, _e) =>
                {
                    popup.Hide();
                    App.Tools.App.WriteLocalSetting(SettingNames.AppVersion, VersionBlock.Version);
                };
                popup.Show();
            }
        }
        /// <summary>
        /// 更改语言首选项
        /// </summary>
        public void LanguageInit()
        {
            string lan = App.Tools.App.GetLocalSetting(SettingNames.Language, "");

            if (lan == "")
            {
                var Languages = Windows.System.UserProfile.GlobalizationPreferences.Languages;
                if (Languages.Count > 0)
                {
                    var language = Languages[0];
                    if (language.ToLower().IndexOf("zh") != -1)
                    {
                        App.Tools.App.WriteLocalSetting(SettingNames.Language, StaticString.LanZh);
                    }
                    else
                    {
                        App.Tools.App.WriteLocalSetting(SettingNames.Language, StaticString.LanEn);
                    }
                }
                else
                {
                    App.Tools.App.WriteLocalSetting(SettingNames.Language, StaticString.LanEn);
                }
            }
            lan = App.Tools.App.GetLocalSetting(SettingNames.Language, StaticString.LanEn);
            string code = "";
            switch (lan)
            {
                case StaticString.LanZh:
                    code = "zh-CN";
                    break;
                case StaticString.LanEn:
                    code = "en-US";
                    break;
                default:
                    code = "en-US";
                    break;
            }
            ApplicationLanguages.PrimaryLanguageOverride = code;
        }

        public async void ShowRestartDialog()
        {
            var dialog = new ConfirmDialog(LanguageNames.NeedRestart);
            dialog.PrimaryButtonClick += async (_s, _e) =>
            {
                await CoreApplication.RequestRestartAsync("restart");
            };
            await dialog.ShowAsync();
        }

        public async Task BackgroundImageInit()
        {
            var list = await App.Tools.IO.GetLocalDataAsync<List<string>>(StaticString.FileImage);
            if (list.Count == 0)
            {
                list.Add("ms-appx:///Assets/Images/desert-castle.jpg");
                list.Add("ms-appx:///Assets/Images/lake.jpg");
                list.Add("ms-appx:///Assets/Images/leaf.jpg");
                list.Add("ms-appx:///Assets/Images/snow-mountain.jpg");
            }
            list.ForEach(p => BackgroundImageCollection.Add(p));
            BackgroundImageToggle();
        }

        public async Task AddBackgroundImage(StorageFile file)
        {
            if (file == null)
                return;
            string guid = Guid.NewGuid().ToString("N");
            string extension = Path.GetExtension(file.Path);
            var folder = ApplicationData.Current.LocalFolder;
            string fileName = $"{guid}.{extension}";
            await file.CopyAsync(folder, fileName, NameCollisionOption.ReplaceExisting);
            BackgroundImageCollection.Add($"ms-appdata:///local/{fileName}");
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileImage, JsonConvert.SerializeObject(BackgroundImageCollection.ToList()));
        }

        public async Task RemoveBackgroundImage(string image)
        {
            BackgroundImageCollection.Remove(image);
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileImage, JsonConvert.SerializeObject(BackgroundImageCollection.ToList()));
        }

        public void BackgroundImageToggle()
        {
            bool isShow = App.Tools.App.GetBoolSetting(SettingNames.EnabledBackgroundImage, false);
            if (isShow)
            {
                string source = App.Tools.App.GetLocalSetting(SettingNames.BackgroundImage, "");
                string color = App.Tools.App.GetLocalSetting(SettingNames.BackgroundMaskColor, App.Current.RequestedTheme == ApplicationTheme.Light ? "#22FFFFFF" : "#22000000");
                if (!string.IsNullOrEmpty(source))
                {
                    MainPage.Current.BackgroundImage.Visibility = Visibility.Visible;
                    MainPage.Current.BackgroundMask.Visibility = Visibility.Visible;
                    MainPage.Current.BackgroundImage.Source = new BitmapImage(new Uri(source));
                    if (!string.IsNullOrEmpty(color))
                    {
                        MainPage.Current.BackgroundMask.Background = new SolidColorBrush(color.Hex16toRGB());
                    }
                }
            }
            else
            {
                MainPage.Current.BackgroundImage.Visibility = Visibility.Collapsed;
                MainPage.Current.BackgroundMask.Visibility = Visibility.Collapsed;
            }
        }
    }
}
