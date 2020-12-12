using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Lib.Share.Models;
using Richasy.Controls.Reader.Models;
using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Clean_Reader.Controls.Components;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using System.Net;
using Richasy.Controls.Reader.Enums;
using Clean_Reader.Controls.Dialogs;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReaderPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        public ObservableCollection<Chapter> ChapterCollection = new ObservableCollection<Chapter>();
        public ObservableCollection<InsideSearchItem> SearchCollection = new ObservableCollection<InsideSearchItem>();
        private Point _lastTouchPoint = new Point(0, 0);
        Book _tempBook = null;
        public static new ReaderPage Current;
        private bool _isTouchItem = false;
        public ReaderPage() : base()
        {
            this.InitializeComponent();
            Current = this;
            IsInit = false;
            vm._reader = ReaderPanel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is Book book)
                {
                    _tempBook = book;
                }
            }
            if (vm.IsXbox)
            {
                CheckPadStatus();
            }
            SystemNavigationManager.GetForCurrentView().BackRequested += BackReuqest;
            Window.Current.Activated += OnWindowActivated;
            base.OnNavigatedTo(e);
        }

        private void BackReuqest(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            vm.CloseReaderView();
        }

        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
                ReaderPanel.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.Activated -= OnWindowActivated;
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackReuqest;
            try
            {
                if (vm.IsDetailChanged)
                    vm.SaveDetailList();
            }
            catch (Exception) { }

            base.OnNavigatingFrom(e);
        }

        private async Task HandleBook(Book book)
        {
            vm.CurrentBook = book;
            BookTitleBlock.Text = book.Name;
            vm.ShowWaitingPopup(LanguageNames.Waiting);
            if (vm._musicPlayer != null)
                vm._musicPlayer.Check();
            var localChapters = await App.VM.GetBookLocalChapters(book.BookId, true);
            if (book.Type == BookType.Web)
            {
                if (localChapters.Count == 0)
                {
                    var chapters = await vm.SyncBookChapters(Convert.ToInt32(book.BookId));
                    if (chapters.Count == 0)
                    {
                        vm.CloseReaderView();
                        return;
                    }
                    vm.CurrentBookChapterList = localChapters;
                    localChapters = chapters;
                }
                var details = await vm.GetBookLocalChapterDetails(book.BookId, true);
                ReaderPanel.LoadCustomView(localChapters, vm.ReaderStyle, details);
            }
            else
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(book.BookId);
                try
                {
                    if (book.Type == BookType.Epub)
                        await ReaderPanel.OpenAsync(file, vm.ReaderStyle, localChapters);
                    else
                        await ReaderPanel.OpenAsync(file, vm.ReaderStyle, localChapters);
                    if (localChapters.Count == 0)
                        await App.VM.SetBookLocalChapters(book.BookId, ReaderPanel.Chapters, true);
                }
                catch (Exception ex)
                {
                    vm.ShowPopup(ex.Message, true);
                }
            }
            vm.HideWaitingPopup();
            ReaderPanel.Focus(FocusState.Programmatic);
        }



        private async void ReaderPanel_OpenCompleted(object sender, EventArgs e)
        {
            LoadingRing.IsActive = false;
            if (!IsInit)
                await Task.Delay(200);
            var localhistory = vm.HistoryList.Where(p => p.BookId == vm.CurrentBook.BookId).FirstOrDefault();
            var cloudHistory = vm.GetCloudHistory(vm.CurrentBook);
            var history = await vm.GetNeedToLoadHistory(localhistory, cloudHistory);
            if (history != null)
                ReaderPanel.LoadHistory(history.Hisotry);
            else
            {
                try
                {
                    ReaderPanel.LoadChapter(ReaderPanel.Chapters.First());
                }
                catch (Exception)
                {
                    await Task.Delay(200);
                    ReaderPanel.LoadChapter(ReaderPanel.Chapters.First());
                }
            }
            ReaderPanel.Focus(FocusState.Programmatic);
        }

        private void ReaderPanel_OpenStarting(object sender, EventArgs e)
        {
            LoadingRing.IsActive = true;
        }

        private void ReaderPanel_ChapterLoaded(object sender, List<Chapter> e)
        {
            vm.CurrentBookChapterList = e;
            LoadChapters();
        }

        public void LoadChapters()
        {
            ChapterCollection.Clear();
            vm.CurrentBookChapterList.ForEach(p => ChapterCollection.Add(p));
        }

        private async void ReaderPanel_ChapterChanged(object sender, Chapter e)
        {
            ChapterListView.SelectedItem = e;
            ChapterListView.ScrollIntoView(e, ScrollIntoViewAlignment.Leading);
            ChapterTitleBlock.Text = e.Title;
            if (App.VM._musicPlayer != null)
            {
                if (App.VM._musicPlayer.IsMediaEnded)
                    await ReaderBar.LoadSpeech();
                else
                    App.VM._musicPlayer.Close();
            }
            ReaderPanel.Focus(FocusState.Programmatic);
        }

        private void ReaderPanel_SetContentStarting(object sender, EventArgs e)
        {
            LoadingRing.IsActive = true;
        }

        private void ReaderPanel_SetContentCompleted(object sender, EventArgs e)
        {
            LoadingRing.IsActive = false;
        }

        private void ReaderPanel_ImageTapped(object sender, ImageEventArgs e)
        {
            _isTouchItem = true;
            if (string.IsNullOrEmpty(e.Tip))
            {
                vm.ShowImagePopup(e.Base64);
            }
            else
            {
                TipTitleBlock.Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Comment);
                TipDescriptionBlock.Text = e.Tip;
                var opt = new FlyoutShowOptions();
                opt.Position = _lastTouchPoint;
                TipFlyout.ShowAt(ReaderPanel, opt);
            }
        }

        private async void ReaderPanel_LinkTapped(object sender, LinkEventArgs e)
        {
            _isTouchItem = true;
            if (!string.IsNullOrEmpty(e.Link))
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            else
            {
                if (!string.IsNullOrEmpty(e.Id))
                {
                    var node = ReaderPanel.GetSpecificIdNode(e.Id, e.FileName);
                    if (node.Name == "body" || node.Name.Contains("h", StringComparison.OrdinalIgnoreCase))
                        ReaderPanel.LocateToSpecificFile(e.FileName);
                    else
                    {
                        var tip = ReaderPanel.GetSpecificIdContent(node, e.Id);
                        TipTitleBlock.Text = tip.Title;
                        TipDescriptionBlock.Text = tip.Description;
                        var opt = new FlyoutShowOptions();
                        opt.Position = _lastTouchPoint;
                        TipFlyout.ShowAt(ReaderPanel, opt);
                    }
                }
                else if (!string.IsNullOrEmpty(e.FileName))
                    ReaderPanel.LocateToSpecificFile(e.FileName);
            }
        }

        private void ReaderPanel_ViewLoaded(object sender, EventArgs e)
        {
            double maxWidth = Convert.ToDouble(App.Tools.App.GetLocalSetting(SettingNames.MaxSingleColumnWidth, "900"));
            ReaderPanel.SingleColumnMaxWidth = maxWidth;
            ReaderBar.Init();
            IsInit = true;
        }

        private void ReaderPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left || e.Key == VirtualKey.GamepadDPadLeft || e.Key == VirtualKey.GamepadLeftThumbstickLeft)
                ReaderPanel.Previous();
            else if (e.Key == VirtualKey.Right || e.Key == VirtualKey.GamepadDPadRight || e.Key == VirtualKey.GamepadLeftThumbstickRight)
                ReaderPanel.Next();
            else if (e.Key == VirtualKey.GamepadMenu)
                ReaderSplitView.IsPaneOpen = !ReaderSplitView.IsPaneOpen;
            else if (e.Key == VirtualKey.GamepadView)
            {
                ReaderBar.Toggle();
                CheckPadStatus();
            }
        }

        public void CheckPadStatus()
        {
            if (ReaderBar.IsShow)
            {
                ReaderBar.Focus(FocusState.Programmatic);
            }
            else
            {
                ReaderPanel.Focus(FocusState.Programmatic);
            }
        }

        private void ReaderPanel_ProgressChanged(object sender, History e)
        {
            var originHistory = vm.HistoryList.Where(p => p.BookId == vm.CurrentBook.BookId).FirstOrDefault();
            if (originHistory != null)
            {
                originHistory.Hisotry = e;
                vm.UpdateCloudHistory(vm.CurrentBook, originHistory);
            }
            else
            {
                var history = new ReadHistory(vm.CurrentBook.BookId, vm.CurrentBook.Name, vm.CurrentBook.Type, e);
                vm.HistoryList.Add(history);
                vm.UpdateCloudHistory(vm.CurrentBook, history);
            }
            vm.IsHistoryChanged = true;
            ProgressBlock.Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Progress) + ": " + e.Progress.ToString("0.0") + "%";
        }

        private void ReaderPanel_TouchTapped(object sender, PositionEventArgs e)
        {
            double width = this.ActualWidth;
            _lastTouchPoint = e.Position;
            if (_isTouchItem)
            {
                _isTouchItem = false;
                return;
            }
            if (e.Position.X > width / 3.0 && e.Position.X < width * 2 / 3.0)
            {
                ReaderBar.Toggle();
                CheckPadStatus();
            }
        }

        private void ChapterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var chapter = e.ClickedItem as Chapter;
            ReaderPanel.LoadChapter(chapter);
            ReaderSplitView.IsPaneOpen = false;
            if (ReaderBar.IsShow)
            {
                ReaderBar.Hide();
                CheckPadStatus();
            }
        }

        private async void ReaderPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ReaderPanel.OpenBeta();
            if (_tempBook != null)
            {
                if (vm._sidePanel.ContainPlayer)
                {
                    vm._sidePanel.RemovePlayer();
                    vm._readerBar.InsertPlayer();
                }
                if (_tempBook != vm.CurrentBook)
                    await HandleBook(_tempBook);

                _tempBook = null;
            }
        }


        private void ReaderBar_BackButtonClick(object sender, RoutedEventArgs e)
        {
            vm.CloseReaderView();
        }

        private void ReaderBar_ChapterButtonClick(object sender, RoutedEventArgs e)
        {
            ReaderSplitView.IsPaneOpen = !ReaderSplitView.IsPaneOpen;
            if (!vm.IsXbox)
            {
                ReaderBar.Hide();
                ReaderPanel.Focus(FocusState.Programmatic);
            }  
        }

        private async void ReaderPanel_CustomContentRequest(object sender, CustomRequestEventArgs e)
        {
            LoadingRing.IsActive = true;
            var detail = await vm.RequestChapterDetail(vm.CurrentBook.BookId, e.RequestChapter);
            if (detail != null)
            {
                ReaderPanel.CustomChapterDetailList.Add(detail);
                ReaderPanel.SetCustomContent(detail, e.StartMode, e.AddonLength);
            }
            LoadingRing.IsActive = false;
            ReaderPanel.Focus(FocusState.Programmatic);
            RequestOtherChapters();
        }

        public async void RequestOtherChapters()
        {
            var currentChapter = ReaderPanel.CurrentChapter;
            var tempChapters = new List<Chapter>();
            int sign = 0;
            for (int i = currentChapter.Index; i < vm.CurrentBookChapterList.Count; i++)
            {
                if (sign > 2)
                    break;
                sign++;
                tempChapters.Add(vm.CurrentBookChapterList[i]);
            }
            sign = 0;
            for (int i = currentChapter.Index - 2; i >= 0; i--)
            {
                if (sign > 2)
                    break;
                sign++;
                tempChapters.Add(vm.CurrentBookChapterList[i]);
            }
            var tasks = new List<Task>();
            foreach (var item in tempChapters)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await vm.RequestChapterDetail(vm.CurrentBook.BookId, item);
                }));
            }
            await Task.WhenAll(tasks.ToArray());
            ReaderPanel.Focus(FocusState.Programmatic);
        }

        private void ReaderContainer_GotFocus(object sender, RoutedEventArgs e)
        {
            ReaderPanel.Focus(FocusState.Programmatic);
        }

        private void ReaderFlyout_Opened(object sender, object e)
        {
            iCiBaBlock.SelectedText = ReaderPanel.SelectedText;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ReaderPanel.SelectedText))
            {
                ReaderFlyout.Hide();
                return;
            }
            var package = new DataPackage();
            package.SetText(ReaderPanel.SelectedText);
            Clipboard.SetContent(package);
            App.VM.ShowPopup(LanguageNames.Copied);
            ReaderFlyout.Hide();
        }

        private void InsideSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchPanel();
        }

        public void ShowSearchPanel()
        {
            if (string.IsNullOrEmpty(ReaderPanel.SelectedText))
            {
                ReaderFlyout.Hide();
                return;
            }
            ShowSearchContainer();
            SearchPanel.Init(ReaderPanel.SelectedText);
            ReaderFlyout.Hide();
        }

        private async void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ReaderPanel.SelectedText))
            {
                ReaderFlyout.Hide();
                return;
            }
            string searchEngine = App.Tools.App.GetLocalSetting(SettingNames.SearchEngine, StaticString.SearchBing);
            string content = WebUtility.UrlEncode(ReaderPanel.SelectedText);
            string url = "";
            switch (searchEngine)
            {
                case StaticString.SearchGoogle:
                    url = $"https://www.google.com/search?q={content}";
                    break;
                case StaticString.SearchBaidu:
                    url = $"https://www.baidu.com/s?wd={content}";
                    break;
                case StaticString.SearchBing:
                    url = $"https://cn.bing.com/search?q={content}";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(url))
            {
                await Launcher.LaunchUriAsync(new Uri(url));
            }
        }

        private async void SearchPanel_QuerySubmit(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                SearchPanel.IsLoading = true;
                SearchPanel.NoDataVisibility = Visibility.Collapsed;
                SearchCollection.Clear();
                try
                {
                    var result = await ReaderPanel.GetInsideSearchResultAsync(e);
                    if (result.Count > 0)
                    {
                        result.ForEach(p => SearchCollection.Add(p));
                    }
                    else
                    {
                        SearchPanel.NoDataVisibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    App.VM.ShowPopup(ex.Message, true);
                }
                SearchPanel.IsLoading = false;
            }
        }

        private void SearchPanel_ItemClick(object sender, InsideSearchItem e)
        {
            HideSearchContainer();
            ReaderPanel.LoadSearchItem(e);
        }

        private void ReaderBar_SearchButtonClick(object sender, RoutedEventArgs e)
        {
            ShowSearchContainer();
        }

        private void SearchContainer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Grid grid && grid.Name == "SearchContainer")
            {
                HideSearchContainer();
            }
        }

        private void ShowSearchContainer()
        {
            SearchContainer.Visibility = Visibility.Visible;
            SearchPanel.Visibility = Visibility.Visible;
        }

        private void HideSearchContainer()
        {
            SearchPanel.Reset();
            SearchCollection.Clear();
            SearchContainer.Visibility = Visibility.Collapsed;
            SearchPanel.Visibility = Visibility.Collapsed;
        }

        private void ReaderPanel_SpeechCueChanged(object sender, SpeechCueEventArgs e)
        {
            if (e.Type == SpeechCueType.Word)
            {
                try
                {
                    ReaderPanel.CheckCurrentReaderIndex(e.SpeechCue.StartPositionInInput);
                }
                catch (Exception)
                { }
            }
        }

        private void ReaderPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _lastTouchPoint = e.GetCurrentPoint(ReaderPanel).Position;
        }
    }
}
