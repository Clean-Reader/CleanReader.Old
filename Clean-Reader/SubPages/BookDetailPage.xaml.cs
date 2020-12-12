using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Richasy.Controls.UWP.Models.UI;
using Richasy.Font.UWP.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Yuenov.SDK.Models.Share;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookDetailPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        private Book _currentBook;
        public BookDetailPage() : base()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null)
            {
                if (e.Parameter is Book webBook)
                {
                    if (webBook != _currentBook)
                        await PageInit(webBook);
                }
            }
            Focus(FocusState.Programmatic);
            base.OnNavigatedTo(e);
        }

        public async Task PageInit(Book data)
        {
            Container.Visibility = Visibility.Collapsed;
            NoDataBlock.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = true;
            _currentBook = data;
            var response = await vm._yuenovClient.GetBookDetailAsync(data.BookId);
            if (response.Result.Code == Yuenov.SDK.Enums.ResultCode.Success)
            {
                var detail = response.Data;
                DetailCard.Data = detail;
                DescriptionBlock.Text = detail.Description;
                RecommendGridView.ItemsSource = detail.Recommend;
                CheckButtonStatus();
                Container.Visibility = Visibility.Visible;
            }
            else
            {
                NoDataBlock.Visibility = Visibility.Visible;
            }
            LoadingRing.IsActive = false;
        }

        private void CheckButtonStatus()
        {
            if (vm.TotalBookList.Any(p => p.BookId == _currentBook.BookId.ToString()))
            {
                AddButton.Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.OpenBook);
                AddButtonIcon.Symbol = FeatherSymbol.BookOpen;
            }
            else
            {
                AddButton.Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.AddToShelf);
                AddButtonIcon.Symbol = FeatherSymbol.Plus;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var source = vm.TotalBookList.Where(p => p.BookId == _currentBook.BookId.ToString()).FirstOrDefault();
            if (source!=null)
            {
                vm.OpenReaderView(source);
            }
            else
            {
                AddButton.IsLoading = true;
                await vm.ImportBook(_currentBook);
                AddButton.IsLoading = false;
                CheckButtonStatus();
            }
        }

        private void RichasyPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainPage.Current.SetSubtitle(LanguageNames.BookDetail);
        }

        private async void RecommendGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Book;
            await PageInit(book);
        }
    }
}
