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
            ShelfCollection.Clear();
            var books = await App.Tools.IO.GetLocalDataAsync<List<Book>>(StaticString.FileShelfList);
            var shelfs = await App.Tools.IO.GetLocalDataAsync<List<EntryItem>>(StaticString.FileShelfIndex);
            var defaultShelf = new EntryItem(App.Tools.App.GetLocalizationTextFromResource(LanguageNames.DefaultShelf),
                                            "default",0);
            shelfs.ForEach(p => p.Count = 0);
            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    if (!string.IsNullOrEmpty(book.ShelfId))
                    {
                        var shelf = shelfs.Where(p => p.Parameter == book.ShelfId).FirstOrDefault();
                        if (shelf == null)
                        {
                            book.ShelfId = "";
                            defaultShelf.Count += 1;
                        }  
                        else
                            shelf.Count += 1;
                    }
                }
            }
            ShelfCollection.Add(defaultShelf);
            shelfs.ForEach(p => ShelfCollection.Add(p));
        }

        public async Task SaveShelf()
        {
            var list = ShelfCollection.Where(p => p.Parameter != "default").ToList();
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileShelfIndex, JsonConvert.SerializeObject(list));
        }
    }
}
