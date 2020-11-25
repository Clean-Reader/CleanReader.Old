using Lib.Share.Enums;
using Lib.Share.Models;
using Richasy.Font.UWP;
using Richasy.Font.UWP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class BookSlimCard : UserControl
    {
        public BookSlimCard()
        {
            this.InitializeComponent();
        }

        public bool IsShowFlyout
        {
            get { return (bool)GetValue(IsShowFlyoutProperty); }
            set { SetValue(IsShowFlyoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowFlyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowFlyoutProperty =
            DependencyProperty.Register("IsShowFlyout", typeof(bool), typeof(BookSlimCard), new PropertyMetadata(false));



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
                instance.FlyoutInit();
                instance.UpdateProgress();
            }
        }

        public Visibility TypeVisibility
        {
            get { return (Visibility)GetValue(TypeVisibilityProperty); }
            set { SetValue(TypeVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeVisibilityProperty =
            DependencyProperty.Register("TypeVisibility", typeof(Visibility), typeof(BookSlimCard), new PropertyMetadata(Visibility.Visible));

        public Visibility ProgressVisibility
        {
            get { return (Visibility)GetValue(ProgressVisibilityProperty); }
            set { SetValue(ProgressVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressVisibilityProperty =
            DependencyProperty.Register("ProgressVisibility", typeof(Visibility), typeof(BookSlimCard), new PropertyMetadata(Visibility.Visible));



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

        private void FlyoutInit()
        {
            CardFlyout.Items.Clear();
            if (IsShowFlyout)
                ContextFlyout = CardFlyout;
            else
            {
                ContextFlyout = null;
                return;
            }
            if (Data.Type == BookType.Web)
            {
                var update = new MenuFlyoutItem()
                {
                    Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.UpdateChapter),
                    Icon = new FeatherIcon(FeatherSymbol.Cloud)
                };
                update.Click += async(_s, _e) =>
                {
                    await App.VM.UpdateBooks(Data);
                };
                CardFlyout.Items.Add(update);
            }
            if (App.VM.ShelfCollection.Count > 1)
            {
                var move = new MenuFlyoutItem()
                {
                    Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Move),
                    Icon = new FeatherIcon(FeatherSymbol.Repeat)
                };
                move.Click += async(_s, _e) =>
                {
                    await App.VM.MoveBook(Data.BookId);
                };
                CardFlyout.Items.Add(move);
            }
            var delete = new MenuFlyoutItem()
            {
                Text = App.Tools.App.GetLocalizationTextFromResource(LanguageNames.Remove),
                Icon = new FeatherIcon(FeatherSymbol.Trash2) { Foreground = App.Tools.App.GetThemeBrushFromResource(ColorNames.ErrorColor) }
            };
            delete.Click += async (_s, _e) =>
            {
                await App.VM.RemoveBook(Data.BookId);
            };
            CardFlyout.Items.Add(delete);
            
        }
    }
}
