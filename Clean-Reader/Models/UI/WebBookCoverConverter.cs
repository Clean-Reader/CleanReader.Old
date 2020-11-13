using Lib.Share.Models;
using System;
using Windows.UI.Xaml.Data;

namespace Clean_Reader.Models.UI
{
    public class WebBookCoverConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var web = value as Yuenov.SDK.Models.Share.Book;
            return new Book(web);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
