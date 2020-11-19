using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Richasy.Controls.UWP.Interaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
    public sealed partial class ReaderBar : UserControl
    {
        AppViewModel vm = App.VM;
        public event RoutedEventHandler BackButtonClick;
        public event RoutedEventHandler ChapterButtonClick;
        public event RoutedEventHandler SearchButtonClick;
        public ReaderBar()
        {
            this.InitializeComponent();
        }
        public void Init()
        {
            ColorConfigPanel.Init();
            FontPanel.Init();
            OtherConfigPanel.Init();
        }
        public void Show()
        {
            MenuBar.Visibility = Visibility.Visible;
        }
        public void Hide()
        {
            MenuBar.Visibility = Visibility.Collapsed;
        }
        public void Toggle()
        {
            MenuBar.Visibility = MenuBar.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        private void ChapterButton_Click(object sender, RoutedEventArgs e)
        {
            ChapterButtonClick?.Invoke(this, e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBar.Visibility = Visibility.Collapsed;
            BackButtonClick?.Invoke(this, e);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBar.Visibility = Visibility.Collapsed;
            SearchButtonClick?.Invoke(this, e);
        }

        private void OtherConfigPanel_MaxSingleColumnChanged(object sender, double e)
        {
            App.VM._reader.SingleColumnMaxWidth = e;
            App.Tools.App.WriteLocalSetting(SettingNames.MaxSingleColumnWidth, e.ToString());
        }

        private async void OtherConfigPanel_CustomRegexSubmit(object sender, Regex e)
        {
            var btn = sender as ActionButton;
            btn.IsLoading = true;
            await App.VM.RebuildChapter(e);
            btn.IsLoading = false;
        }

        private void OtherFlyout_Opened(object sender, object e)
        {
            OtherConfigPanel.Init();
        }
    }
}
