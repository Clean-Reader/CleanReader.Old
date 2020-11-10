using Clean_Reader.Models.Core;
using Clean_Reader.Models.Enums;
using Clean_Reader.Models.UI;
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
        private double ParentHeight
        {
            get => App.VM._sidePanel.NavigateRow.ActualHeight;
        }

        public NavigateMenu()
        {
            this.InitializeComponent();
        }

        private async void CollapseItem_HeaderTapped(object sender, EventArgs e)
        {
            var container = sender as CollapseItem;
            var items = Container.Children.OfType<CollapseItem>();
            foreach (var item in items)
            {
                if (!item.Equals(container))
                    item.IsExpand = false;
            }
            Enum.TryParse(container.Tag.ToString(), out GroupType type);
            if (type != GroupType.Setting && type != GroupType.Discovery)
            {
                await HandleExpand(type);
                container.IsExpand = !container.IsExpand;
                await Task.Delay(50);
                int rowIndex = Grid.GetRow(container);
                for (int i = 0; i < Container.RowDefinitions.Count; i++)
                {
                    if (rowIndex == i && Container.ActualHeight > ParentHeight && container.IsExpand)
                        Container.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
                    else
                        Container.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Auto);
                }
            }
            else
            {
                UnselectItems(vm.StoreCollection, vm.ShelfCollection);
                // show setting page
            }
        }

        private async Task HandleExpand(GroupType type)
        {
            switch (type)
            {
                case GroupType.Shelf:
                    await ShelfHandle();
                    break;
                case GroupType.Store:
                    await StoreHandle();
                    break;
                default:
                    break;
            }
        }

        private async Task ShelfHandle()
        {
            if (App.VM.ShelfCollection.Count == 0)
            {
                ShelfContainer.GoToLoading();
                await App.VM.ShelfInit();
                ShelfContainer.GoToLoading(false);
            }
            if (vm.ShelfCollection.Count == 1 && !vm.ShelfCollection.First().IsSelected)
            {
                vm.ShelfCollection.First().IsSelected = true;
                ShelfContainer.InnerListView.SelectedItem = vm.ShelfCollection.First();
                // goto shelf
            }
        }

        private async Task StoreHandle()
        {
            if (App.VM.StoreCollection.Count == 0)
            {
                StoreContainer.GoToLoading();
                await App.VM.WebCategoriesInit();
                StoreContainer.GoToLoading(false);
            }
        }

        public void CheckLayout()
        {
            var container = Container.Children.OfType<CollapseItem>().Where(p => p.IsExpand).FirstOrDefault();

            if (container != null)
            {
                int rowIndex = Grid.GetRow(container);
                for (int i = 0; i < Container.RowDefinitions.Count; i++)
                {
                    if (rowIndex == i && Container.ActualHeight > ParentHeight)
                        Container.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
                    else
                        Container.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Auto);
                }
            }
        }

        private void Collapse_ItemClick(object sender, ItemClickEventArgs e)
        {
            var container = sender as CollapseItem;
            var item = e.ClickedItem as EntryItem;
            switch (item.GroupType)
            {
                case GroupType.Shelf:
                    UnselectItems(vm.StoreCollection);
                    //Load Shelf Page
                    break;
                case GroupType.Store:
                    UnselectItems(vm.ShelfCollection);
                    //Load Store Page
                    break;
                default:
                    break;
            }
        }

        private void UnselectItems(params ObservableCollection<EntryItem>[] collections)
        {
            foreach (var collection in collections)
            {
                foreach (var item in collection)
                {
                    item.IsSelected = false;
                }
            }
        }
    }
}
