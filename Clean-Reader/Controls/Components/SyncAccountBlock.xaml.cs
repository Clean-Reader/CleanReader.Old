using Clean_Reader.Controls.Dialogs;
using Lib.Share.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SyncAccountBlock : UserControl
    {
        private bool _isLoaded = false;
        public SyncAccountBlock()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                LoginButton.IsLoading = true;
                await App.VM.OneDriveInit();
                LoginButton.IsLoading = false;
                await Init();
                _isLoaded = true;
            }
        }

        private async Task Init()
        {
            string token = App.Tools.App.GetLocalSetting(SettingNames.OneDriveAccessToken, "");
            bool isEmpty = string.IsNullOrEmpty(token);
            LoginButton.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
            LogoutButton.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
            string name = App.Tools.App.GetLocalSetting(SettingNames.OneDriveUserName, "");
            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(token))
            {
                Avatar.Visibility = Visibility.Collapsed;
                LoadingRing.IsActive = true;
                var user = await App.VM._onedrive.GetMeAsync();
                if (user != null)
                {
                    App.Tools.App.WriteLocalSetting(SettingNames.OneDriveUserName, user.DisplayName);
                    name = user.DisplayName;
                }
                else
                {
                    App.VM.ShowPopup(LanguageNames.GetUserFailed, true);
                }
            }
            Avatar.DisplayName = name;
            if (string.IsNullOrEmpty(name))
                name = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.NeedLogin);
            Avatar.Visibility = Visibility.Visible;
            LoadingRing.IsActive = false;
            UserNameBlock.Text = name;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsLoading = true;
            try
            {
                var result = await App.VM._onedrive.AuthorizationAsync();
                if (result != null)
                {
                    App.Tools.App.WriteLocalSetting(SettingNames.OneDriveAccessToken, result.AccessToken);
                    App.Tools.App.WriteLocalSetting(SettingNames.OneDriveExpiryTime, App.Tools.App.DateToTimeStamp(result.ExpiresOn.DateTime).ToString());
                    await Init();
                }
                else
                {
                    App.VM.ShowPopup(LanguageNames.AuthorizationCanceled, true);
                }
            }
            catch (Exception ex)
            {
                App.VM.ShowPopup(ex.Message, true);
            }
            LoginButton.IsLoading = false;
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmDialog(LanguageNames.OneDriveLogoutWarning);
            dialog.PrimaryButtonClick += async(_s, _e) =>
            {
                App.Tools.App.WriteLocalSetting(SettingNames.OneDriveAccessToken, "");
                App.Tools.App.WriteLocalSetting(SettingNames.OneDriveExpiryTime, "");
                App.Tools.App.WriteLocalSetting(SettingNames.OneDriveUserName, "");
                await App.VM.OneDriveInit();
                await Init();
            };
            await dialog.ShowAsync();
        }
    }
}
