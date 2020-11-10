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
    }
}
