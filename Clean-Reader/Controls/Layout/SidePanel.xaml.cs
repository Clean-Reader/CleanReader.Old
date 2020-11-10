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
            var files = await App.Tools.IO.OpenLocalFilesAsync(".epub", ".txt");
            if (files != null && files.Count > 0)
            {

            }
            ImportButton.IsLoading = false;
        }
    }
}
