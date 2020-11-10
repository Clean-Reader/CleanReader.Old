using Lib.Share.Enums;
using System.Collections.Generic;

namespace Lib.Share.Models
{
    public class Book
    {
        public string ShelfId { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public BookType Type { get; set; }
        public string BookId { get; set; }

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
