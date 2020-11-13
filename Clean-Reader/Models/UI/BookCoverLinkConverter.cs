using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Clean_Reader.Models.UI
{
    public class BookCoverLinkConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return App.VM._yuenovClient.GetImageUrl(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
