﻿using Clean_Reader.Controls.Dialogs;
using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Lib.Share.Models;
using Richasy.Controls.UWP.Models.UI;
using Richasy.Font.UWP;
using Richasy.Font.UWP.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShelfPage : RichasyPage
    {
        public AppViewModel vm = App.VM;
        public ShelfPage()
        {
            this.InitializeComponent();
            vm.LastestReadCollection.CollectionChanged += LastestReadCollection_Changed;
            vm.DisplayBookCollection.CollectionChanged += DisplayBookCollection_Changed;
            LastestContainer.Visibility = vm.LastestReadCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            NoDataBlock.Visibility = vm.DisplayBookCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ToolTipService.SetToolTip(AddShelfButton, App.Tools.App.GetLocalizationTextFromResource(LanguageNames.AddShelf));
            ToolTipService.SetToolTip(ManageShelfButton, App.Tools.App.GetLocalizationTextFromResource(LanguageNames.ManageShelf));
            vm.CurrentShelfChanged += CurrentShelfChanged;
            ShelfNameBlock.Text = vm.CurrentShelf?.Name ?? "--";
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Focus(FocusState.Programmatic);
            base.OnNavigatedTo(e);
        }

        private void CurrentShelfChanged(object sender, EventArgs e)
        {
            ShelfNameBlock.Text = vm.CurrentShelf?.Name ?? "--";
        }

        private void DisplayBookCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoDataBlock.Visibility = vm.DisplayBookCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LastestReadCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            LastestContainer.Visibility = vm.LastestReadCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            try
            {
                await vm.ImportBooks();
            }
            catch (Exception ex)
            {
                vm.ShowPopup(ex.Message, true);
            }
            
            btn.IsEnabled = true;
        }

        private void BookView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Book;
            vm.OpenReaderView(book);
        }

        private async void AddShelfButton_Click(object sender, RoutedEventArgs e)
        {
            ShelfFlyout.Hide();
            var dialog = new ShelfDialog();
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Secondary)
                vm.CurrentShelfInit();
        }

        private async void ManageShelfButton_Click(object sender, RoutedEventArgs e)
        {
            if (vm.CurrentShelf.Id == "default")
            {
                vm.ShowPopup(LanguageNames.DefaultShelfManageWarning);
                return;
            }
            ShelfFlyout.Hide();
            var dialog = new ShelfDialog(vm.CurrentShelf);
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Secondary)
                vm.CurrentShelfInit();
        }

        private void ShelfFlyout_Opened(object sender, object e)
        {
            ShelfFlyout.SecondaryCommands.Clear();
            foreach (var shelf in vm.ShelfCollection)
            {
                var btn = new AppBarToggleButton();
                btn.MinWidth = 200;
                btn.Label = shelf.Name;
                btn.Tag = shelf.Id;
                btn.Click += (_s, _e) =>
                {
                    if (vm.CurrentShelf != shelf)
                        vm.CurrentShelf = shelf;
                    ShelfFlyout.Hide();
                };
                if (shelf.Id == vm.CurrentShelf.Id)
                    btn.IsChecked = true;
                ShelfFlyout.SecondaryCommands.Add(btn);
            }
        }

        private void LastestReadListView_Loaded(object sender, RoutedEventArgs e)
        {
            LastestReadListView.Focus(FocusState.Programmatic);
        }
    }
}
