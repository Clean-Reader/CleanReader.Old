﻿using Clean_Reader.Models.Core;
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
using Richasy.Font.UWP;
using Clean_Reader.Models.UI;
using Newtonsoft.Json;
using Clean_Reader.Controls.Components;
using Windows.UI;

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
        Book _tempBook = null;
        public ReaderPage() : base()
        {
            this.InitializeComponent();
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
            UpdateHeaderFooterStyle();
            base.OnNavigatedTo(e);
        }

        private async Task HandleBook(Book book)
        {
            vm.CurrentBook = book;
            BookTitleBlock.Text = book.Name;
            if (book.Type == BookType.Web)
            {

            }
            else
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(book.BookId);
                try
                {
                    if (book.Type == BookType.Epub)
                        await ReaderPanel.OpenAsync(file, vm.ReaderStyle);
                    else
                        await ReaderPanel.OpenAsync(file, vm.ReaderStyle);
                }
                catch (Exception ex)
                {

                }

            }
            ReaderPanel.Focus(FocusState.Programmatic);
        }

        private async void ReaderPanel_OpenCompleted(object sender, EventArgs e)
        {
            LoadingRing.IsActive = false;
            if (!IsInit)
                await Task.Delay(200);
            var history = vm.HistoryList.Where(p => p.BookId == vm.CurrentBook.BookId).FirstOrDefault();
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
        }

        private void ReaderPanel_OpenStarting(object sender, EventArgs e)
        {
            LoadingRing.IsActive = true;
        }

        private void ReaderPanel_ChapterLoaded(object sender, List<Chapter> e)
        {
            ChapterCollection.Clear();
            e.ForEach(p => ChapterCollection.Add(p));
        }

        private void ReaderPanel_ChapterChanged(object sender, Chapter e)
        {
            ChapterListView.SelectedItem = e;
            ChapterListView.ScrollIntoView(e, ScrollIntoViewAlignment.Leading);
            ChapterTitleBlock.Text = e.Title;
        }

        private void ReaderPanel_SetContentStarting(object sender, EventArgs e)
        {
            LoadingRing.IsActive = true;
        }

        private void ReaderPanel_SetContentCompleted(object sender, EventArgs e)
        {
            LoadingRing.IsActive = false;
        }

        private async void ReaderPanel_ImageTapped(object sender, ImageEventArgs e)
        {
            var byteArray = Convert.FromBase64String(e.Base64);
            var stream = byteArray.AsBuffer().AsStream().AsRandomAccessStream();
            using (stream)
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                // do other thing
            }
        }

        private async void ReaderPanel_LinkTapped(object sender, LinkEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Link))
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            else
            {
                if (!string.IsNullOrEmpty(e.Id))
                {
                    var node = ReaderPanel.GetSpecificIdNode(e.Id, e.FileName);
                    if (node.Name == "body")
                        ReaderPanel.LocateToSpecificFile(e.FileName);
                    else
                    {
                        var tip = ReaderPanel.GetSpecificIdContent(node, e.Id);
                        await new MessageDialog(tip.Description, tip.Title).ShowAsync();
                    }
                }
                else if (!string.IsNullOrEmpty(e.FileName))
                    ReaderPanel.LocateToSpecificFile(e.FileName);
            }
        }

        private void ReaderPanel_ViewLoaded(object sender, EventArgs e)
        {
            ReaderBar.Init();
            IsInit = true;
        }

        private void ReaderPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left)
                ReaderPanel.Previous();
            else if (e.Key == VirtualKey.Right)
                ReaderPanel.Next();
        }

        private void ReaderPanel_ProgressChanged(object sender, History e)
        {
            var originHistory = vm.HistoryList.Where(p => p.BookId == vm.CurrentBook.BookId).FirstOrDefault();
            if (originHistory != null)
                originHistory.Hisotry = e;
            else
                vm.HistoryList.Add(new ReadHistory(vm.CurrentBook.BookId, e));
            vm.IsHistoryChanged = true;
            ProgressBlock.Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Progress) + ": " + e.Progress.ToString("0.0") + "%";
        }

        private void ReaderPanel_TouchTapped(object sender, PositionEventArgs e)
        {
            double width = this.ActualWidth;
            if (e.Position.X > width / 3.0 && e.Position.X < width * 2 / 3.0)
            {
                ReaderBar.Toggle();
                ReaderPanel.Focus(FocusState.Programmatic);
            }
        }

        private void ChapterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var chapter = e.ClickedItem as Chapter;
            ReaderPanel.LoadChapter(chapter);
        }

        private async void ReaderPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tempBook != null)
            {
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
            ReaderBar.Hide();
            ReaderPanel.Focus(FocusState.Programmatic);
        }

        public void UpdateHeaderFooterStyle()
        {
            ReaderHeaderContainer.Background = ReaderFooterContainer.Background = vm.GetBackgroundBrush();
        }
    }
}