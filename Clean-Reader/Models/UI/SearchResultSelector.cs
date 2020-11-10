using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Clean_Reader.Models.UI
{
    public class SearchResultSelector : DataTemplateSelector
    {
        public DataTemplate WebResultTemplate { get; set; }
        public DataTemplate LocalResultTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var result = item as SearchResult;
            if (result.Book == null)
                return WebResultTemplate;
            else
                return LocalResultTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var result = item as SearchResult;
            if (result.Book == null)
                return WebResultTemplate;
            else
                return LocalResultTemplate;
        }
    }
}
