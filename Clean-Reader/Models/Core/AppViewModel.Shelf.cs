using Clean_Reader.Controls.Dialogs;
using Clean_Reader.Models.UI;
using Lib.Share.Enums;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.Reader.Models.Epub;
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
                await shelfDialog.ShowAsync();
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
                    var coverFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers",CreationCollisionOption.OpenIfExists);
                    var coverFile = await coverFolder.CreateFileAsync(book.BookId + ".png", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(coverFile, epub.CoverImage);
                }
                TotalBookList.Add(book);
            }
            string currentShelfId = CurrentShelf.Id == "default" ? "" : CurrentShelf.Id;
            if (currentShelfId == shelfId)
                CurrentShelfInit();
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileShelfList, JsonConvert.SerializeObject(TotalBookList));
        }

        public async Task SaveShelf()
        {
            var list = ShelfCollection.Where(p => p.Id != "default").ToList();
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileShelfIndex, JsonConvert.SerializeObject(list));
        }
    }
}
