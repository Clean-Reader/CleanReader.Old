using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Richasy.Controls.Reader.Models;

namespace Lib.Share.Models
{
    public class ReadHistory
    {
        public string BookId { get; set; }
        public History Hisotry { get; set; }
        public ReadHistory(string id, History history)
        {
            BookId = id;
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
