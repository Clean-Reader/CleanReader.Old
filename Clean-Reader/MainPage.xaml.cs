using Richasy.Controls.UWP.Models.UI;
using Clean_Reader.Models.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Clean_Reader.Models.Enums;
using Clean_Reader.Models.UI;
using Lib.Share.Enums;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Clean_Reader
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : RichasyPage
    {
        public AppViewModel vm = App.VM;
        public new static MainPage Current;
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            vm._rootFrame = MainFrame;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void RichasyPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInit)
            {
                await vm.BackgroundImageInit();
                vm.CheckUpdate();
                await vm.ShelfInit();
                vm.ViewStyleInit();
                vm.ColorConfigInit();
                vm.HistoryInit();
                vm._menu.Navigate(new MenuItem(MenuItemType.Shelf));
                await vm._yuenovClient.WarmUpAsync();
                IsInit = true;
            }

            // TODO
        }

        private void RichasyPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            if (width <= vm._narrowBreakpoint && AppSplitView.DisplayMode == SplitViewDisplayMode.CompactInline)
            {
                AppSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                SideMenuButton.Visibility = Visibility.Visible;
                AppSearchBox.Width = 180;
                Container.Padding = new Thickness(25, 10, 25, 0);
            }
            else if (width > vm._narrowBreakpoint && AppSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                AppSplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                AppSplitView.IsPaneOpen = true;
                SideMenuButton.Visibility = Visibility.Collapsed;
                AppSearchBox.Width = 300;
                Container.Padding = new Thickness(45, 20, 45, 0);
            }
        }

        private void SideMenuButton_Click(object sender, RoutedEventArgs e)
        {
            AppSplitView.IsPaneOpen = !AppSplitView.IsPaneOpen;
        }

        public void NavigateSubPage(Type pageType, object para = null)
        {
            vm.SubFrameHistoryList.RemoveAll(p => p.Item1.Equals(pageType));
            vm.SubFrameHistoryList.Add(new Tuple<Type, object>(pageType, para));
            SubFrame.Navigate(pageType, para, new DrillInNavigationTransitionInfo());
            SecondarySplitView.IsPaneOpen = true;
            SideBackButton.Visibility = vm.SubFrameHistoryList.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetSubtitle(string title)
        {
            SubtitleBlock.Text = title;
        }
        public void SetSubtitle(LanguageNames title)
        {
            SubtitleBlock.Text = App.Tools.App.GetLocalizationTextFromResource(title);
        }

        private void SideBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SubFrameHistoryList.Count > 1)
            {
                var last = vm.SubFrameHistoryList[vm.SubFrameHistoryList.Count-2];
                SubFrame.Navigate(last.Item1, last.Item2);
                vm.SubFrameHistoryList.RemoveAt(vm.SubFrameHistoryList.Count - 1);
            }
            SideBackButton.Visibility = vm.SubFrameHistoryList.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
