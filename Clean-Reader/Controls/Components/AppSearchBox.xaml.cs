using Clean_Reader.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class AppSearchBox : UserControl
    {
        private ObservableCollection<SearchResult> ResultCollection = new ObservableCollection<SearchResult>();
        public AppSearchBox()
        {
            this.InitializeComponent();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string text = sender.Text;
            if (!string.IsNullOrEmpty(text))
            {
                if (ResultCollection.Count == 0)
                    ResultCollection.Add(new SearchResult(text));
                var first = ResultCollection.First();
                if (first.SearchText != text)
                    first.SearchText = text;
                var books = App.VM.TotalBookList.Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
                if (books.Count > 0)
                {
                    if (ResultCollection.Count > 1)
                    {
                        for (int i = ResultCollection.Count - 1; i > 0; i--)
                        {
                            if (!books.Contains(ResultCollection[i].Book))
                                ResultCollection.RemoveAt(i);
                        }
                    }
                    foreach (var item in books)
                    {
                        if (!ResultCollection.Any(p => item.BookId == p.Book?.BookId))
                            ResultCollection.Add(new SearchResult("", item));
                    }
                }
                else
                {
                    for (int i = ResultCollection.Count - 1; i > 0; i--)
                    {
                        ResultCollection.RemoveAt(i);
                    }
                }
            }
            else
            {
                ResultCollection.Clear();
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var item = args.ChosenSuggestion as SearchResult;
            if (item != null)
            {
                if (item.Book == null)
                {
                    //search in web
                    MainPage.Current.NavigateSubPage(typeof(SubPages.SearchDetailPage), item.SearchText);
                }
                else
                {
                    // open the book
                    App.VM.OpenReaderView(item.Book);
                }
            }
        }
    }
}
