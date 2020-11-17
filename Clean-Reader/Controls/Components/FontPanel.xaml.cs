using Clean_Reader.Models.Core;
using Richasy.Font.UWP;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class FontPanel : UserControl
    {
        private bool IsInit = false;
        AppViewModel vm = App.VM;
        public FontPanel()
        {
            this.InitializeComponent();
        }
        public void Init()
        {
            IsInit = false;
            FontSizeSlider.Value = vm.ReaderStyle.FontSize;
            TextIndentSlider.Value = vm.ReaderStyle.TextIndent;
            LineHeightSlider.Value = vm.ReaderStyle.LineHeight;
            SegmentSpacingSlider.Value = vm.ReaderStyle.SegmentSpacing;
            if (vm.FontCollection.Count == 0)
            {
                var fonts = SystemFont.GetSystemFonts().OrderBy(p => p.Name).ToList();
                fonts.ForEach(p => vm.FontCollection.Add(p));
            }
            FontListView.SelectedItem = vm.FontCollection.Where(p => p.Name == vm.ReaderStyle.FontFamily).FirstOrDefault();
            IsInit = true;
        }
        private void FontListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var font = e.ClickedItem as SystemFont;
            vm.ChangeReaderFont(font);
        }

        private void FontSizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsInit)
            {
                double value = e.NewValue;
                vm.ChangeReaderFontSize(value);
            }
        }

        private void TextIndentSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsInit)
            {
                vm.ReaderStyle.TextIndent = e.NewValue;
                vm.UpdateStyle();
            }
        }

        private void LineHeightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsInit)
            {
                vm.ReaderStyle.LineHeight = e.NewValue;
                vm.UpdateStyle();
            }
        }

        private void SegmentSpacingSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsInit)
            {
                vm.ReaderStyle.SegmentSpacing = e.NewValue;
                vm.UpdateStyle();
            }
        }
    }
}
