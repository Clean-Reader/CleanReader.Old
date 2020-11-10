using Clean_Reader.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuenov.SDK.Enums;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public async Task WebCategoriesInit()
        {
            var response = await _yuenovClient.GetTotalCategoriesAsync();
            if (response.Result.Code == ResultCode.Success)
            {
                WebChannels = response.Data.Channels;
                var categories = WebChannels.SelectMany(p => p.Categories);
                foreach (var cate in categories)
                {
                    if (!StoreCollection.Any(p => p.Name == cate.CategoryName))
                    {
                        var item = new EntryItem(cate.CategoryName);
                        item.Parameter = cate.CategoryId.ToString();
                        StoreCollection.Add(item);
                    }
                }
            }
        }
    }
}
