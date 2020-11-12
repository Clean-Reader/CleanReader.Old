using Lib.Share.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class BookSlimCard : UserControl
    {
        public BookSlimCard()
        {
            this.InitializeComponent();
        }

        public Book Data
        {
            get { return (Book)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Book), typeof(BookSlimCard), new PropertyMetadata(null,new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null && e.NewValue is Book data)
            {
                var instance = d as BookSlimCard;
                instance.NameBlock.Text = data.Name;
                instance.TypeBlock.Text = data.Type.ToString().ToUpper();
                instance.Cover.Data = data;
                App.VM.ProgressChanged -= instance.OnProgressChanged;
                App.VM.ProgressChanged += instance.OnProgressChanged;
                instance.UpdateProgress();
            }
        }

        private void OnProgressChanged(object sender, EventArgs e)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (Data == null)
                return;
            var history = App.VM.HistoryList.Where(p => p.BookId == Data.BookId).FirstOrDefault();
            if (history != null)
                ProgressBlock.Text = history.Hisotry.Progress.ToString("0") + "%";
            else
                ProgressBlock.Text = "0%";
        }
    }
}
