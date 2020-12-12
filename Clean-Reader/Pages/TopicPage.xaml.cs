using Clean_Reader.Models.Core;
using Lib.Share.Enums;
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
using Yuenov.SDK.Models.Discovery;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TopicPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        public TopicPage():base()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (vm.TopicCollection.Count == 0)
                await PageInit();
            Focus(FocusState.Programmatic);
            base.OnNavigatedTo(e);
        }

        private async Task PageInit()
        {
            LoadingRing.IsActive = true;
            NoDataBlock.Visibility = Visibility.Collapsed;
            await vm.TopicInit();
            if (vm.TopicCollection.Count == 0)
                NoDataBlock.Visibility = Visibility.Visible;
            else
                NoDataBlock.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }

        private void HorizonBookListView_ItemClick(object sender, Yuenov.SDK.Models.Share.Book e)
        {
            MainPage.Current.NavigateSubPage(typeof(SubPages.BookDetailPage), e);
        }

        private void HorizonBookListView_AllButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as SpecialContainer;
            MainPage.Current.NavigateSubPage(typeof(SubPages.TopicDetailPage), data);
        }
    }
}
