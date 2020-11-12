using Richasy.Controls.UWP.Models.UI;
using Clean_Reader.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Clean_Reader.Models.Enums;
using Clean_Reader.Models.UI;

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
                vm.CheckUpdate();
                await vm.ShelfInit();
                vm.ViewStyleInit();
                vm.ColorConfigInit();
                vm.HistoryInit();
                vm._menu.Navigate(new MenuItem(MenuItemType.Shelf));
                IsInit = true;
            }
           
            // TODO
        }

        private void RichasyPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            // TODO
        }
    }
}
