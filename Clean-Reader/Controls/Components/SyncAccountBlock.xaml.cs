using Lib.Share.Enums;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class SyncAccountBlock : UserControl
    {
        public SyncAccountBlock()
        {
            this.InitializeComponent();
            Init();
        }

        private void Init()
        {
            string token = App.Tools.App.GetLocalSetting(SettingNames.OneDriveAccessToken, "");
            bool isEmpty = string.IsNullOrEmpty(token);
            LoginButton.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
            DetailContainer.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
