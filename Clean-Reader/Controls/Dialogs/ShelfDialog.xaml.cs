using Lib.Share.Enums;
using Lib.Share.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Controls.Dialogs
{
    public sealed partial class ShelfDialog : ContentDialog
    {
        private ObservableCollection<Book> BookCollection = new ObservableCollection<Book>();
        private Shelf _source = null;
        public ShelfDialog()
        {
            this.InitializeComponent();
            Title = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.AddShelf);
            PrimaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Confirm);
            SecondaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Cancel);
            BookListView.Header = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.UncateBookList);
            var list = App.VM.TotalBookList.Where(p => string.IsNullOrEmpty(p.ShelfId) || p.ShelfId == "default").ToList();
            list.ForEach(p => BookCollection.Add(p));
            CheckListStatus();
        }

        public ShelfDialog(Shelf shelf)
        {
            this.InitializeComponent();
            _source = shelf;
            BookListView.Header = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.ShelfBookList);
            PrimaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Confirm);
            SecondaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Cancel);
            Title = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.ManageShelf);
            ShelfNameBox.Text = shelf.Name;
            var list = App.VM.TotalBookList.Where(p => p.ShelfId == shelf.Id).ToList();
            list.ForEach(p => BookCollection.Add(p));
            BookListView.SelectAll();
            CheckListStatus();
        }

        private void CheckListStatus()
        {
            NoDataBlock.Visibility = BookCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            if (string.IsNullOrEmpty(ShelfNameBox.Text))
            {
                App.VM.ShowPopup(LanguageNames.FieldEmpty, true);
                return;
            }
            var repeat = App.VM.ShelfCollection.Where(p => p.Name.Equals(ShelfNameBox.Text)).FirstOrDefault();
            if (repeat != null)
            {
                if (_source == null || (_source != null && _source.Id != repeat.Id))
                {
                    App.VM.ShowPopup(LanguageNames.ShelfNameRepeat, true);
                    return;
                }
            }
            IsPrimaryButtonEnabled = false;
            PrimaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Waiting);
            var temp = new List<Book>();
            var selectedItems = BookListView.SelectedItems;
            if (selectedItems.Count > 0)
            {
                foreach (var item in selectedItems)
                {
                    temp.Add(item as Book);
                }
            }
            if (_source != null)
            {
                var sourceBooks = App.VM.TotalBookList.Where(p => p.ShelfId == _source.Id).ToList();
                foreach (var book in sourceBooks)
                {
                    if (!temp.Contains(book))
                        book.ShelfId = "";
                }
                _source.Name = ShelfNameBox.Text;
            }
            else
            {
                var shelf = new Shelf(ShelfNameBox.Text);
                foreach (var item in temp)
                {
                    var source = App.VM.TotalBookList.Where(p => p.BookId == item.BookId).FirstOrDefault();
                    if (source != null)
                        source.ShelfId = shelf.Id;
                }
                App.VM.ShelfCollection.Add(shelf);
            }
            App.VM.IsShelfChanged = true;
            await App.VM.SaveShelf();
            IsPrimaryButtonEnabled = false;
            PrimaryButtonText = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Confirm);
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void BookListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_source != null && BookCollection.Count > 0)
            {
                BookListView.SelectAll();
            }
        }
    }
}
