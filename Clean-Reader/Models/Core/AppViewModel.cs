using System;
using Lib.Share.Enums;
using Yuenov.SDK;
using Richasy.Helper.UWP;
using System.Threading.Tasks;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.UWP.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Richasy.Font.UWP;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.System;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Clean_Reader.Controls.Dialogs;

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

        public async void AccelertorKeyActivedHandle(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var esc = Window.Current.CoreWindow.GetKeyState(VirtualKey.Escape);
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                var shift = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
                if (esc.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (IsReaderPage)
                    {
                        CloseReaderView();
                    }
                }
                else if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (args.VirtualKey == VirtualKey.Q)
                    {
                        IList<AppDiagnosticInfo> infos = await AppDiagnosticInfo.RequestInfoForAppAsync();
                        IList<AppResourceGroupInfo> resourceInfos = infos[0].GetResourceGroups();
                        await resourceInfos[0].StartSuspendAsync();
                    }
                    else if (args.VirtualKey == VirtualKey.F)
                    {
                        if (IsReaderPage)
                        {
                            Pages.ReaderPage.Current.ShowSearchPanel();
                        }
                    }
                }
            }
        }

        private async void CheckFileTimer_Tick(object sender, object e)
        {
            if (IsHistoryChanged)
            {
                IsHistoryChanged = false;
                if (IsOneDriveInit && !string.IsNullOrEmpty(_oneDriveHistoryFileId))
                    await _onedrive.UpdateFileAsync(_oneDriveHistoryFileId, JsonConvert.SerializeObject(CloudHistoryList));
                await App.Tools.IO.SetLocalDataAsync(StaticString.FileHistory, JsonConvert.SerializeObject(HistoryList));
            }
            if (_isStyleChanged)
            {
                _isStyleChanged = false;
                await App.Tools.IO.SetLocalDataAsync(StaticString.FileReaderStyle, JsonConvert.SerializeObject(ReaderStyle));
            }
            if (IsDetailChanged)
                SaveDetailList();
            if (IsBookListChanged)
            {
                IsBookListChanged = false;
                await App.Tools.IO.SetLocalDataAsync(StaticString.FileShelfList, JsonConvert.SerializeObject(TotalBookList));
            }
        }

        public async void SaveDetailList()
        {
            IsDetailChanged = false;
            await App.Tools.IO.SetLocalDataAsync(CurrentBook.BookId + ".json", JsonConvert.SerializeObject(CurrentBookChapterDetailList), StaticString.FolderChapterDetail);
        }

        public async Task OneDriveInit()
        {
            IsOneDriveInit = false;
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
                            IsOneDriveInit = true;
                            App.Tools.App.WriteLocalSetting(SettingNames.OneDriveAccessToken, result.AccessToken);
                            App.Tools.App.WriteLocalSetting(SettingNames.OneDriveExpiryTime, App.Tools.App.DateToTimeStamp(result.ExpiresOn.DateTime).ToString());
                        }
                    }
                    catch (Exception)
                    { }
                }
                else
                    IsOneDriveInit = true;
            }
        }

        public void WaitingPopupInit()
        {
            _waitPopup = new WaitingPopup(App.Tools);
            _waitPopup.PopupBackground = new SolidColorBrush(Colors.Transparent);
            _waitPopup.PresenterBackground = App.Tools.App.GetThemeBrushFromResource(ColorNames.PopupBackground);
            _waitPopup.ProgressRingStyle = App.Tools.App.GetStyleFromResource(StyleNames.BasicProgressRingStyle);
            _waitPopup.TextStyle = App.Tools.App.GetStyleFromResource(StyleNames.BodyTextStyle);
        }
        public ReadHistory GetCloudHistory(Book book)
        {
            var cloudHistory = CloudHistoryList.Where(p => p.BookId == CurrentBook.BookId || (p.BookName == book.Name && p.Type == book.Type)).FirstOrDefault();
            return cloudHistory;
        }

        public async Task<ReadHistory> GetNeedToLoadHistory(ReadHistory local, ReadHistory cloud)
        {
            if (cloud == null || cloud.Hisotry.Time <= local.Hisotry.Time)
                return local;
            else
            {
                ReadHistory history = local;
                var dialog = new ConfirmDialog(LanguageNames.ChooseReadHistory);
                dialog.PrimaryButtonClick += (_s, _e) =>
                {
                    history = cloud;
                };
                await dialog.ShowAsync();
                return history;
            }
        }
        public void UpdateCloudHistory(Book book, ReadHistory history)
        {
            var cloud = GetCloudHistory(book);
            if (cloud != null)
                cloud.Hisotry = history.Hisotry;
            else
                CloudHistoryList.Add(history);
        }
    }
}
