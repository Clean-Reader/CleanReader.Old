using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace Clean_Reader.Models.UI
{
    public class LevelFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var level = (int)value;
            if (level == 1)
                return FontWeights.Bold;
            else
                return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
