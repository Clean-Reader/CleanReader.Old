using Clean_Reader.Models.Core;
using Richasy.Controls.UWP.Models.UI;
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

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoryPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        public CategoryPage() : base()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (vm.CategoryCollection.Count == 0 || vm.RankCollection.Count == 0)
                await PageInit();
            Focus(FocusState.Programmatic);
            base.OnNavigatedTo(e);
        }

        private async Task PageInit()
        {
            CategoryLoadingBar.Visibility = Visibility.Visible;
            RankLoadingBar.Visibility = Visibility.Visible;
            CategoryNoDataBlock.Visibility = Visibility.Collapsed;
            await vm.WebCategoriesInit();
            if (vm.CategoryCollection.Count == 0)
                CategoryNoDataBlock.Visibility = Visibility.Visible;
            else
                CategoryNoDataBlock.Visibility = Visibility.Collapsed;

            CategoryLoadingBar.Visibility = Visibility.Collapsed;

            await vm.RankInit();
            if (vm.RankCollection.Count == 0)
                RankNoDataBlock.Visibility = Visibility.Visible;
            else
                RankNoDataBlock.Visibility = Visibility.Collapsed;

            RankLoadingBar.Visibility = Visibility.Collapsed;
            
        }

        private void CategoryGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var cate = e.ClickedItem as Category;
            MainPage.Current.NavigateSubPage(typeof(SubPages.CategoryDetailPage), cate);
        }

        private void RankGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rank = e.ClickedItem as Rank;
            MainPage.Current.NavigateSubPage(typeof(SubPages.RankDetailPage), rank);
        }
    }
}
