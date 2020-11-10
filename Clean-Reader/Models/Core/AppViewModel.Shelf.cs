using Clean_Reader.Models.UI;
using Lib.Share.Enums;
using Lib.Share.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public async Task ShelfInit()
        {
            var books = await App.Tools.IO.GetLocalDataAsync<List<Book>>(StaticString.FileShelfList);
            var shelfs = await App.Tools.IO.GetLocalDataAsync<List<Shelf>>(StaticString.FileShelfIndex);
            var defaultShelf = new Shelf(App.Tools.App.GetLocalizationTextFromResource(LanguageNames.DefaultShelf),"default");
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
            string lastOpenShelfId = App.Tools.App.GetLocalSetting(SettingNames.LastShelfId, "default");
            var lastShelf = shelfs.Where(p => p.Id == lastOpenShelfId).FirstOrDefault();
            if (lastShelf != null)
                CurrentShelf = lastShelf;
            else
                CurrentShelf = shelfs.First();
        }

        public async Task SaveShelf()
        {
            var list = ShelfCollection.Where(p => p.Id != "default").ToList();
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileShelfIndex, JsonConvert.SerializeObject(list));
        }
    }
}
