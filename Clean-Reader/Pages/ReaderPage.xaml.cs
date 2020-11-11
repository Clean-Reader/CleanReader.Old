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
        public ReaderPage():base()
        {
            this.InitializeComponent();
            vm._reader = ReaderPanel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if(e.Parameter is Book book)
                {
                    await HandleBook(book);
                }
            }
            base.OnNavigatedTo(e);
        }

        private async Task HandleBook(Book book)
        {
            vm.CurrentBook = book;
            if (book.Type == BookType.Web)
            {

            }
            else
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(book.BookId);
                if (book.Type == BookType.Epub)
                    await ReaderPanel.OpenAsync(file, vm._epubViewStyle);
                else
                    await ReaderPanel.OpenAsync(file, vm._txtViewStyle);
            }
        }

        private void ReaderPanel_OpenCompleted(object sender, System.EventArgs e)
        {
            LoadingRing.IsActive = false;
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

        }

        private void ReaderPanel_SetContentStarting(object sender, EventArgs e)
        {
            LoadingRing.IsActive = true;
        }

        private void ReaderPanel_SetContentCompleted(object sender, EventArgs e)
        {
            LoadingRing.IsActive = false;
        }

        private async void ReaderPanel_ImageTapped(object sender, Richasy.Controls.Reader.Models.ImageEventArgs e)
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

        private async void ReaderPanel_LinkTapped(object sender, Richasy.Controls.Reader.Models.LinkEventArgs e)
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
            var history = vm.HistoryList.Where(p => p.BookId == vm.CurrentBook.BookId).FirstOrDefault();
            if (history != null)
                ReaderPanel.LoadHistory(history.Hisotry);
            else
                ReaderPanel.LoadChapter(ReaderPanel.Chapters.First());
        }

        private void ReaderPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left)
                ReaderPanel.Previous();
            else if (e.Key == VirtualKey.Right)
                ReaderPanel.Next();
        }

        private void ReaderPanel_ProgressChanged(object sender, Richasy.Controls.Reader.Models.History e)
        {
            var originHistory = vm.HistoryList.Where(p => p.BookId == vm.CurrentBook.BookId).FirstOrDefault();
            if (originHistory != null)
                originHistory.Hisotry = e;
            else
                vm.HistoryList.Add(new ReadHistory(vm.CurrentBook.BookId, e));
            vm.IsHistoryChanged = true;
        }

        private void ReaderPanel_TouchTapped(object sender, PositionEventArgs e)
        {

        }

        private void ChapterListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
