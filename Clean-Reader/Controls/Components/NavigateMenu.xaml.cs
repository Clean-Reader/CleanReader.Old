using Clean_Reader.Models.Core;
using Clean_Reader.Models.Enums;
using Clean_Reader.Models.UI;
using Richasy.Helper.UWP.Extension;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class NavigateMenu : UserControl
    {
        public AppViewModel vm = App.VM;
        public ObservableCollection<MenuItem> MenuItemCollection = new ObservableCollection<MenuItem>();

        public NavigateMenu()
        {
            this.InitializeComponent();
            vm._menu = this;
            var items = MenuItem.GetMenuItems();
            items.ForEach(p => MenuItemCollection.Add(p));
        }

        private void MenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MenuItem;
            Navigate(item);
        }

        public void Navigate(MenuItem item)
        {
            MainPage.Current.TitleBlock.Text = item.Name;
            if (MenuListView.SelectedItem != item)
                MenuListView.SelectedItem = item;
            vm.NavigateToPage(item.Type);
        }

        private void MenuListView_Loaded(object sender, RoutedEventArgs e)
        {
            var items = MenuListView.VisualTreeFindAll<ListViewItemPresenter>();
            var first = items.First();
            var last = items.Last();
            first.CornerRadius = new CornerRadius(10, 10, 0, 0);
            last.CornerRadius = new CornerRadius(0, 0, 10, 10);
            var lastContainer = last.VisualTreeFindName<Grid>("Container");
            lastContainer.BorderThickness = new Thickness(0);
        }
    }
}
