using Clean_Reader.Controls.Components;
using Clean_Reader.Controls.Layout;
using Clean_Reader.Models.UI;
using Lib.Share.Models;
using Richasy.Controls.Reader;
using Richasy.Helper.UWP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Yuenov.SDK;
using Yuenov.SDK.Models.Share;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public YuenovClient _yuenovClient;

        public ReaderPanel _reader;
        public SidePanel _sidePanel;
        public NavigateMenu _menu;
        public Frame _rootFrame;

        private const string _clientId = "4ce94634-4e8d-4e7a-9967-18c59afd1dc7";
        private string[] _scopes= new string[] { "Files.ReadWrite.AppFolder", "User.Read" };
        public OneDriveHelper _onedrive;

        public List<Lib.Share.Models.Book> TotalBookList = new List<Lib.Share.Models.Book>();

        public ObservableCollection<Shelf> ShelfCollection = new ObservableCollection<Shelf>();

        private Shelf _currentShelf;
        public Shelf CurrentShelf
        {
            get => _currentShelf;
            set
            {
                if(_currentShelf==null || !_currentShelf.Equals(value))
                {
                    _currentShelf = value;
                    CurrentShelfChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ObservableCollection<Lib.Share.Models.Book> DisplayBookCollection = new ObservableCollection<Lib.Share.Models.Book>();
        public ObservableCollection<Lib.Share.Models.Book> LastestReadCollection = new ObservableCollection<Lib.Share.Models.Book>();

        public List<Category> WebCategories = new List<Category>();

        public event EventHandler CurrentShelfChanged;
    }
}
