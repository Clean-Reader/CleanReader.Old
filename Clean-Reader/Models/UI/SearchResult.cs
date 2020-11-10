using Lib.Share.Models;
using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Reader.Models.UI
{
    public class SearchResult:NotifyPropertyBase
    {
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => Set(ref _searchText, value);
        }
        public Book Book { get; set; }
        public SearchResult()
        {

        }

        public SearchResult(string text, Book book = null)
        {
            SearchText = text;
            Book = book;
        }

        public override string ToString()
        {
            if (Book != null)
                return Book.Name;
            else
                return SearchText;
        }
    }
}
