using Clean_Reader.Models.Core;
using Lib.Share.Enums;
using Richasy.Controls.UWP.Models.UI;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscoveryPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        public DiscoveryPage():base()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (vm.DiscoveryCollection.Count == 0)
                await PageInit();
            base.OnNavigatedTo(e);
        }

        private async Task PageInit()
        {
            LoadingRing.IsActive = true;
            await vm.DiscoveryInit();
            if (vm.DiscoveryCollection.Count == 0)
                NoDataBlock.Visibility = Visibility.Visible;
            else
                NoDataBlock.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }

        private void HorizonBookListView_ItemClick(object sender, Yuenov.SDK.Models.Share.Book e)
        {
            MainPage.Current.NavigateSubPage(typeof(SubPages.BookDetailPage),LanguageNames.BookDetail, e);
        }

        private void HorizonBookListView_AllButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
