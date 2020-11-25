using Clean_Reader.Controls.Dialogs;
using Clean_Reader.Models.UI;
using Lib.Share.Enums;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.Reader.Enums;
using Richasy.Controls.Reader.Models;
using Richasy.Controls.Reader.Models.Epub;
using Richasy.Font.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public async Task ShelfInit()
        {
            var books = await App.Tools.IO.GetLocalDataAsync<List<Book>>(StaticString.FileShelfList);
            var shelfs = await App.Tools.IO.GetLocalDataAsync<List<Shelf>>(StaticString.FileShelfIndex);
            var lastest = await App.Tools.IO.GetLocalDataAsync<List<string>>(StaticString.FileLastestList);
            var defaultShelf = new Shelf(App.Tools.App.GetLocalizationTextFromResource(LanguageNames.DefaultShelf), "default");
            ShelfCollection.Clear();
            LastestReadCollection.Clear();
            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    if (!string.IsNullOrEmpty(book.ShelfId))
                    {
                        var shelf = shelfs.Where(p => p.Id == book.ShelfId).FirstOrDefault();
                        if (shelf == null)
                        {
                            book.ShelfId = "";
                        }
                    }
                }
            }
            TotalBookList = books;
            ShelfCollection.Add(defaultShelf);
            shelfs.ForEach(p => ShelfCollection.Add(p));
            foreach (var lastId in lastest)
            {
                var book = books.Where(p => p.BookId == lastId).FirstOrDefault();
                if (book != null)
                    LastestReadCollection.Add(book);
            }
            string lastOpenShelfId = App.Tools.App.GetLocalSetting(SettingNames.LastShelfId, "default");
            var lastShelf = shelfs.Where(p => p.Id == lastOpenShelfId).FirstOrDefault();
            if (lastShelf != null)
                CurrentShelf = lastShelf;
            else
                CurrentShelf = ShelfCollection.First();
        }

        public async Task ImportBooks()
        {
            var files = await App.Tools.IO.OpenLocalFilesAsync(".epub", ".txt");
            if (files == null || files.Count() == 0)
                return;
            string shelfId = "";
            if (ShelfCollection.Count > 1)
            {
                var shelfDialog = new ShelfSelectionDialog(CurrentShelf);
                shelfDialog.PrimaryButtonClick += (_s, _e) =>
                {
                    if (shelfDialog.SelectedItem != null)
                        shelfId = shelfDialog.SelectedItem.Id == "default" ? "" : shelfDialog.SelectedItem.Id;
                };
                var cr = await shelfDialog.ShowAsync();
                if (cr == Windows.UI.Xaml.Controls.ContentDialogResult.None)
                    return;
            }
            foreach (var file in files)
            {
                var book = new Book(file, shelfId);
                if (TotalBookList.Contains(book))
                {
                    ShowPopup($"{App.Tools.App.GetLocalizationTextFromResource(LanguageNames.RepeatBook)}:{book.Name}", true);
                    continue;
                }
                if (book.Type == BookType.Epub)
                {
                    var epub = await EpubReader.Read(file, Encoding.Default);
                    var coverFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
                    var coverFile = await coverFolder.CreateFileAsync(book.BookId + ".png", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(coverFile, epub.CoverImage);
                }
                TotalBookList.Add(book);
            }
            string currentShelfId = CurrentShelf.Id == "default" ? "" : CurrentShelf.Id;
            if (currentShelfId == shelfId)
                CurrentShelfInit();
            IsBookListChanged = true;
        }

        public async Task ImportBook(Yuenov.SDK.Models.Share.Book web)
        {
            string shelfId = "";
            var book = new Book(web);
            if (TotalBookList.Contains(book))
            {
                ShowPopup($"{App.Tools.App.GetLocalizationTextFromResource(LanguageNames.RepeatBook)}:{book.Name}", true);
                return;
            }
            if (ShelfCollection.Count > 1)
            {
                var shelfDialog = new ShelfSelectionDialog(CurrentShelf);
                shelfDialog.PrimaryButtonClick += (_s, _e) =>
                {
                    if (shelfDialog.SelectedItem != null)
                        shelfId = shelfDialog.SelectedItem.Id == "default" ? "" : shelfDialog.SelectedItem.Id;
                };
                var cr = await shelfDialog.ShowAsync();
                if (cr == Windows.UI.Xaml.Controls.ContentDialogResult.None)
                    return;
            }
            TotalBookList.Add(book);
            ShowPopup(LanguageNames.LoadingChapter);
            await SyncBookChapters(web.BookId);
            string currentShelfId = CurrentShelf.Id == "default" ? "" : CurrentShelf.Id;
            if (currentShelfId == shelfId)
                CurrentShelfInit();
            IsBookListChanged = true;
            ShowPopup(LanguageNames.AddSuccess);
        }

        public async Task SaveShelf()
        {
            var list = ShelfCollection.Where(p => p.Id != "default").ToList();
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileShelfIndex, JsonConvert.SerializeObject(list));
        }

        public async void HistoryInit()
        {
            var history = await App.Tools.IO.GetLocalDataAsync<List<ReadHistory>>(StaticString.FileHistory);
            HistoryList = history;
            ProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        public void CurrentShelfInit()
        {
            string shelfId = CurrentShelf.Id == "default" ? "" : CurrentShelf.Id;
            var books = TotalBookList.Where(p => p.ShelfId == shelfId).OrderByDescending(p => p.CreateTime).ToList();
            DisplayBookCollection.Clear();
            foreach (var book in books)
            {
                if (!DisplayBookCollection.Contains(book))
                    DisplayBookCollection.Add(book);
            }
        }

        public async Task RemoveBook(string bookId, bool showDialog = true)
        {
            if (showDialog)
            {
                var dialog = new ConfirmDialog(LanguageNames.RemoveBookWarning);
                var result = await dialog.ShowAsync();
                if (result != Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                    return;
            }
            var source = TotalBookList.Where(p => p.BookId == bookId).FirstOrDefault();
            if (source != null)
            {
                TotalBookList.Remove(source);
                if (DisplayBookCollection.Contains(source))
                    DisplayBookCollection.Remove(source);
                if (LastestReadCollection.Contains(source))
                {
                    LastestReadCollection.Remove(source);
                    await App.Tools.IO.SetLocalDataAsync(StaticString.FileLastestList, JsonConvert.SerializeObject(LastestReadCollection.Select(p => p.BookId)));
                }
                await App.Tools.IO.SetLocalDataAsync(bookId + ".json", "[]", StaticString.FolderChapter);
                HistoryList.RemoveAll(p => p.BookId == bookId);
                _isHistoryChanged = true;
                IsBookListChanged = true;
            }
        }

        public async Task MoveBook(string bookId)
        {
            var source = TotalBookList.Where(p => p.BookId == bookId).FirstOrDefault();
            if (source != null)
            {
                var parentShelf = ShelfCollection.Where(p => p.Id == source.ShelfId).FirstOrDefault();
                var dialog = new ShelfSelectionDialog(parentShelf);
                dialog.PrimaryButtonClick += (_s, _e) =>
                {
                    var selectedShelf = dialog.SelectedItem;
                    if (selectedShelf != parentShelf)
                    {
                        dialog.IsPrimaryButtonEnabled = false;
                        source.ShelfId = selectedShelf.Id;
                        CurrentShelfInit();
                        IsBookListChanged = true;
                    }
                };
                await dialog.ShowAsync();
            }
        }
    }
}
