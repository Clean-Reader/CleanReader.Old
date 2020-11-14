using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Yuenov.SDK.Enums;
using Yuenov.SDK.Models.Share;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoryDetailPage : RichasyPage
    {
        private ObservableCollection<Book> DisplayCollection = new ObservableCollection<Book>();
        private List<Book> NewestList = new List<Book>();
        private List<Book> HotList = new List<Book>();
        private List<Book> EndList = new List<Book>();
        private int newestIndex = 1;
        private int hotIndex = 1;
        private int endIndex = 1;
        AppViewModel vm = App.VM;
        private Category _category;
        private bool _isRequesting = false;
        public CategoryDetailPage()
        {
            this.InitializeComponent();
        }

        private void Reset()
        {
            IsInit = false;
            NewestList.Clear();
            EndList.Clear();
            HotList.Clear();
            DisplayCollection.Clear();
            newestIndex = 1;
            hotIndex = 1;
            endIndex = 1;
            SortTypeComboBox.SelectedIndex = 0;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is Category cate)
            {
                if (cate != _category)
                {
                    _category = cate;
                    Reset();
                    await RequestData();
                }
            }
            base.OnNavigatedTo(e);
        }

        public async Task RequestData()
        {
            CategoryNameBlock.Text = _category.CategoryName;
            MainPage.Current.SetSubtitle(LanguageNames.Category);
            if (_isRequesting)
                return;
            _isRequesting = true;
            if (DisplayCollection.Count == 0)
                LoadingRing.IsActive = true;
            string sign = (SortTypeComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            int index = 0;
            Enum.TryParse(sign, out SortType type);
            switch (type)
            {
                case SortType.Newest:
                    index = newestIndex;
                    break;
                case SortType.Hot:
                    index = hotIndex;
                    break;
                case SortType.End:
                    index = endIndex;
                    break;
                default:
                    break;
            }
            if (index > 1)
                LoadingBar.Visibility = Visibility.Visible;
            var response = await vm._yuenovClient.GetCategoryDetailAsync(_category.CategoryId, type, 0, index);
            if (response.Result.Code == ResultCode.Success)
            {
                var data = response.Data.List;
                switch (type)
                {
                    case SortType.Newest:
                        NewestList.AddRange(data);
                        newestIndex += 1;
                        break;
                    case SortType.Hot:
                        HotList.AddRange(data);
                        hotIndex += 1;
                        break;
                    case SortType.End:
                        EndList.AddRange(data);
                        endIndex += 1;
                        break;
                    default:
                        break;
                }
                data.ForEach(p => DisplayCollection.Add(p));
            }
            _isRequesting = false;
            LoadingRing.IsActive = false;
            LoadingBar.Visibility = Visibility.Collapsed;
            IsInit = true;
        }

        private async void DetailListView_ArriveBottom(object sender, EventArgs e)
        {
            if(IsInit)
                await RequestData();
        }

        private void DetailListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Book;
            MainPage.Current.NavigateSubPage(typeof(BookDetailPage), book);
        }

        private void RichasyPage_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryNameBlock.Text = _category.CategoryName;
            MainPage.Current.SetSubtitle(LanguageNames.Category);
        }

        private async void SortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayCollection.Clear();
            Enum.TryParse((SortTypeComboBox.SelectedItem as ComboBoxItem).Tag.ToString(), out SortType type);
            switch (type)
            {
                case SortType.Newest:
                    NewestList.ForEach(p => DisplayCollection.Add(p));
                    break;
                case SortType.Hot:
                    HotList.ForEach(p => DisplayCollection.Add(p));
                    break;
                case SortType.End:
                    EndList.ForEach(p => DisplayCollection.Add(p));
                    break;
                default:
                    break;
            }
            if (IsInit)
            {
                IsInit = false;
                await RequestData();
            }
        }
    }
}
