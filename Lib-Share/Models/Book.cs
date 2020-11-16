using Lib.Share.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Lib.Share.Models
{
    public class Book
    {
        public string ShelfId { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public BookType Type { get; set; }
        public string BookId { get; set; }
        public string CustomRegex { get; set; }
        public DateTime CreateTime { get; set; }
        public long LastChapterId { get; set; }

        public Book() { }

        public Book(StorageFile file, string shelfId = "")
        {
            string ext = Path.GetExtension(file.Path);
            ShelfId = shelfId;
            Name = file.DisplayName.Replace(ext, "");
            Type = ext.Equals(".txt", System.StringComparison.OrdinalIgnoreCase) ? BookType.Txt : BookType.Epub;
            string id = StorageApplicationPermissions.FutureAccessList.Add(file);
            BookId = id;
            if (Type == BookType.Epub)
                Cover = $"ms-appdata:///local/Covers/{id}.png";
            CreateTime = DateTime.Now;
        }

        public Book(Yuenov.SDK.Models.Share.Book web, string shelfId = "", long lastChapterId = 0)
        {
            ShelfId = shelfId;
            Name = web.Title;
            Cover = web.CoverImg;
            Type = BookType.Web;
            BookId = web.BookId.ToString();
            CreateTime = DateTime.Now;
            LastChapterId = lastChapterId;
        }
        public override bool Equals(object obj)
        {
            return obj is Book book &&
                   BookId == book.BookId;
        }

        public override int GetHashCode()
        {
            return -1590218067 + EqualityComparer<string>.Default.GetHashCode(BookId);
        }
    }
}
