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
using Yuenov.SDK.Models.Discovery;
using Yuenov.SDK.Models.Share;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscoveryDetailPage : RichasyPage
    {
        public ObservableCollection<Book> DisplayCollection = new ObservableCollection<Book>();
        private int index = 1;
        AppViewModel vm = App.VM;
        private DiscoveryContainer _container;
        private bool _isRequesting = false;
        public DiscoveryDetailPage() : base()
        {
            this.InitializeComponent();
        }
        private void Reset()
        {
            IsInit = false;
            DisplayCollection.Clear();
            index = 1;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is DiscoveryContainer con)
            {
                if (con != _container)
                {
                    _container = con;
                    Reset();
                    await RequestData();
                }
            }
            base.OnNavigatedTo(e);
        }

        public async Task RequestData()
        {
            MainPage.Current.SetSubtitle(_container.CategoryName);
            if (_isRequesting)
                return;
            _isRequesting = true;
            if (DisplayCollection.Count == 0)
                LoadingRing.IsActive = true;
            if (index > 1)
                LoadingBar.Visibility = Visibility.Visible;
            var response = await vm._yuenovClient.GetDiscoveryDetailAsync(_container.Type, index, categoryId: _container.CategoryId);
            if (response.Result.Code == ResultCode.Success)
            {
                var data = response.Data.List;
                index += 1;
                data.ForEach(p => DisplayCollection.Add(p));
            }
            _isRequesting = false;
            LoadingRing.IsActive = false;
            LoadingBar.Visibility = Visibility.Collapsed;
            IsInit = true;
        }

        private async void DetailListView_ArriveBottom(object sender, EventArgs e)
        {
            if (IsInit)
                await RequestData();
        }

        private void DetailListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Book;
            MainPage.Current.NavigateSubPage(typeof(BookDetailPage), book);
        }
    }
}
