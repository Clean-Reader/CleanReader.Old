using Clean_Reader.Controls.Components;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Layout
{
    public sealed partial class SidePanel : UserControl
    {
        public SidePanel()
        {
            this.InitializeComponent();
            App.VM._sidePanel = this;
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ImportButton.IsLoading = true;
            await App.VM.ImportBooks();
            ImportButton.IsLoading = false;
        }

        public bool ContainPlayer
        {
            get => Container.Children.Contains(App.VM._musicPlayer);
        }

        public void InsertPlayer()
        {
            Container.Children.Add(App.VM._musicPlayer);
            Grid.SetRow(App.VM._musicPlayer, 3);
        }

        public void RemovePlayer()
        {
            Grid.SetRow(App.VM._musicPlayer, 0);
            Container.Children.Remove(App.VM._musicPlayer);
        }
    }
}
