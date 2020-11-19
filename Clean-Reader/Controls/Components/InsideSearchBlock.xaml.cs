using Lib.Share.Enums;
using Richasy.Controls.Reader.Models;
using System;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class InsideSearchBlock : UserControl
    {
        public InsideSearchBlock()
        {
            this.InitializeComponent();
        }
        public InsideSearchItem Data
        {
            get { return (InsideSearchItem)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(InsideSearchItem), typeof(InsideSearchBlock), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is InsideSearchItem data)
            {
                var instance = d as InsideSearchBlock;
                instance.ChapterNameBlock.Text = data.Chapter.Title;
                instance.DisplayTextBlock.Inlines.Clear();
                var sp = data.DisplayText.Split(data.SearchText);
                instance.DisplayTextBlock.Inlines.Add(new Run { Text = sp.First() });
                instance.DisplayTextBlock.Inlines.Add(new Run { Text = data.SearchText, FontWeight = FontWeights.Bold,
                Foreground=App.Tools.App.GetThemeBrushFromResource(ColorNames.PrimaryColor)});
                if (sp.Length > 1)
                    instance.DisplayTextBlock.Inlines.Add(new Run { Text = sp.Last() });
            }
        }
    }
}
