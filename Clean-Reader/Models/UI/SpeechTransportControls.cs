using Clean_Reader.Controls.Components;
using Lib.Share.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Clean_Reader.Models.UI
{
    public sealed class SpeechTransportControls : MediaTransportControls
    {
        private TextBlock _bookNameBlock;
        private TextBlock _typeBlock;
        private BookCover _bookCover;
        private AppBarButton _saveButton;

        public event RoutedEventHandler SaveButtonClick;

        protected override void OnApplyTemplate()
        {
            _bookNameBlock = GetTemplateChild("BookNameBlock") as TextBlock;
            _typeBlock = GetTemplateChild("TypeBlock") as TextBlock;
            _bookCover = GetTemplateChild("BookCover") as BookCover;
            _saveButton = GetTemplateChild("SaveButton") as AppBarButton;

            _saveButton.Click += (_s, _e) => { SaveButtonClick?.Invoke(_s, _e); };

            base.OnApplyTemplate();
        }

        public string BookName
        {
            get { return (string)GetValue(BookNameProperty); }
            set { SetValue(BookNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BookName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookNameProperty =
            DependencyProperty.Register("BookName", typeof(string), typeof(SpeechTransportControls), new PropertyMetadata(null));

        public Book Book
        {
            get { return (Book)GetValue(BookProperty); }
            set { SetValue(BookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Book.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookProperty =
            DependencyProperty.Register("Book", typeof(Book), typeof(SpeechTransportControls), new PropertyMetadata(null,new PropertyChangedCallback(Book_Changed)));

        private static void Book_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null && e.NewValue is Book data)
            {
                var instance = d as SpeechTransportControls;
                instance._bookCover.Data = data;
                instance._bookNameBlock.Text = data.Name;
                instance._typeBlock.Text = data.Type.ToString().ToUpper();
            }
        }

        public string ChapterName
        {
            get { return (string)GetValue(ChapterNameProperty); }
            set { SetValue(ChapterNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChapterName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChapterNameProperty =
            DependencyProperty.Register("ChapterName", typeof(string), typeof(SpeechTransportControls), new PropertyMetadata(null));
    }
}
