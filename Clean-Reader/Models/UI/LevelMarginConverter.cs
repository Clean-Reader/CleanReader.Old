using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Clean_Reader.Models.UI
{
    public class LevelMarginCovnerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var level = (int)value;
            if (level == 0)
                return new Thickness(10, 0, 0, 0);
            else
                return new Thickness(((level - 1) * 20) + 10, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
