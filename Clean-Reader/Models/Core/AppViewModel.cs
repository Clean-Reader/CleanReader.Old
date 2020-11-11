using System;
using Lib.Share.Enums;
using Yuenov.SDK;
using Richasy.Helper.UWP;
using System.Threading.Tasks;
using Lib.Share.Models;
using Newtonsoft.Json;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public AppViewModel()
        {
            _yuenovClient = new YuenovClient();
            _yuenovClient.SetOpenToken("e89309f4-6cd8-4a45-90de-922e7d71455a");
            CurrentShelfChanged += CurrentShelf_Changed;
            _checkFileTimer.Tick += CheckFileTimer_Tick;
            _checkFileTimer.Start();
        }

        private async void CheckFileTimer_Tick(object sender, object e)
        {
            if (IsHistoryChanged)
            {
                IsHistoryChanged = false;
                await App.Tools.IO.SetLocalDataAsync(StaticString.FileHistory, JsonConvert.SerializeObject(HistoryList));
            }
        }

        public async Task OneDriveInit()
        {
            string token = App.Tools.App.GetLocalSetting(SettingNames.OneDriveAccessToken, "");
            if (string.IsNullOrEmpty(token))
                _onedrive = new OneDriveHelper(_clientId, _scopes);
            else
            {
                _onedrive = new OneDriveHelper(_clientId, _scopes, token);
                int now = App.Tools.App.GetNowSeconds();
                int expiry = Convert.ToInt32(App.Tools.App.GetLocalSetting(SettingNames.OneDriveExpiryTime, "0"));
                if (now >= expiry)
                {
                    try
                    {
                        var result = await _onedrive.RefreshTokenAsync();
                        if (result != null)
                        {
                            App.Tools.App.WriteLocalSetting(SettingNames.OneDriveAccessToken, result.AccessToken);
                            App.Tools.App.WriteLocalSetting(SettingNames.OneDriveExpiryTime, App.Tools.App.DateToTimeStamp(result.ExpiresOn.DateTime).ToString());
                        }
                    }
                    catch (Exception)
                    {}
                    
                }
            }
        }
    }
}
