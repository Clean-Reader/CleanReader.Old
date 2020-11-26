using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Share.Enums;
using Richasy.Controls.Reader.Models;

namespace Lib.Share.Models
{
    public class ReadHistory
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public BookType Type { get; set; }
        public History Hisotry { get; set; }
        public ReadHistory(string id,string name,BookType type, History history)
        {
            BookId = id;
            BookName = name;
            Type = type;
            Hisotry = history;
        }

        public override bool Equals(object obj)
        {
            return obj is ReadHistory history &&
                   BookId == history.BookId;
        }

        public override int GetHashCode()
        {
            return -1590218067 + EqualityComparer<string>.Default.GetHashCode(BookId);
        }
    }
}
