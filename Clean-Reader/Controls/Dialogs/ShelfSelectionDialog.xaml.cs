using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Lib.Share.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Controls.Dialogs
{
    public sealed partial class ShelfSelectionDialog : ContentDialog
    {
        public AppViewModel vm = App.VM;
        public Shelf SelectedItem
        {
            get => ShelfComboBox.SelectedItem as Shelf;
        }
        public ShelfSelectionDialog()
        {
            this.InitializeComponent();
            Title = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.ShelfSelection);
            PrimaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Confirm);
            CloseButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Cancel);
        }

        public ShelfSelectionDialog(Shelf selectedShelf) : this()
        {
            if (selectedShelf != null)
                ShelfComboBox.SelectedItem = selectedShelf;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
