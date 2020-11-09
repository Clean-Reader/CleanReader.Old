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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace Clean_Reader.Controls.Dialogs
{
    public sealed partial class ConfirmDialog : ContentDialog
    {
        public ConfirmDialog()
        {
            this.InitializeComponent();
            Title = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Warning);
            PrimaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Confirm);
            CloseButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Cancel);
        }
        public ConfirmDialog(string msg) : this()
        {
            DisplayBlock.Text = msg;
        }
        public ConfirmDialog(LanguageNames language) : this()
        {
            DisplayBlock.Text = App.Tools.App.GetLocalizationTextFromResource(language);
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
