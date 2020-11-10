using Lib.Share.Enums;
using Lib.Share.Models;
using Richasy.Controls.UWP.Popups;
using Richasy.Controls.UWP.Widgets;
using Richasy.Font.UWP;
using Richasy.Font.UWP.Enums;
using System;
using Windows.Globalization;
using Windows.Storage;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public void ShowPopup(LanguageNames name, bool isError = false)
        {
            ShowPopup(App.Tools.App.GetLocalizationTextFromResource(name), isError);
        }
        public void ShowPopup(string msg, bool isError = false)
        {
            var popup = new TipPopup(App.Tools, msg);
            ColorNames color = isError ? ColorNames.ErrorColor : ColorNames.PrimaryColor;
            popup.Show(color);
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
    }
}
