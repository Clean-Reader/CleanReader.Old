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
                CategoryCollection.Clear();
                var categories = response.Data.Channels.SelectMany(p => p.Categories);
                foreach (var cate in categories)
                {
                    if (!CategoryCollection.Any(p => p.CategoryName == cate.CategoryName))
                    {
                        CategoryCollection.Add(cate);
                    }
                }
            }
        }

        public async Task DiscoveryInit()
        {
            DiscoveryCollection.Clear();
            try
            {
                var discoveryResponse = await _yuenovClient.GetDiscoveryPageAsync();
                if (discoveryResponse.Result.Code == ResultCode.Success)
                {
                    var data = discoveryResponse.Data.List;
                    if (data != null)
                        data.ForEach(p => DiscoveryCollection.Add(p));
                }
                else
                    App.VM.ShowPopup($"{discoveryResponse.Result.Code}: {discoveryResponse.Result.Message}");
            }
            catch (Exception ex)
            {
                App.VM.ShowPopup(ex.Message, true);
            }
        }
        public async Task TopicInit()
        {
            TopicCollection.Clear();
            try
            {
                var specialResponse = await _yuenovClient.GetAllSpecialListAsync();
                if (specialResponse.Result.Code == ResultCode.Success)
                {
                    var data = specialResponse.Data.SpecialList;
                    if (data != null)
                        data.ForEach(p => TopicCollection.Add(p));
                }
                else
                    App.VM.ShowPopup($"{specialResponse.Result.Code}: {specialResponse.Result.Message}");
            }
            catch (Exception ex)
            {
                App.VM.ShowPopup(ex.Message, true);
            }
        }

        public async Task RankInit()
        {
            RankCollection.Clear();
            try
            {
                var response = await _yuenovClient.GetTotalRanksAsync();
                if (response.Result.Code == ResultCode.Success)
                {
                    var ranks = response.Data.Channels.SelectMany(p => p.Ranks);
                    foreach (var rank in ranks)
                    {
                        if (!RankCollection.Any(p => p.RankName == rank.RankName))
                        {
                            RankCollection.Add(rank);
                        }
                    }
                }
                else
                    App.VM.ShowPopup($"{response.Result.Code}: {response.Result.Message}");
            }
            catch (Exception ex)
            {
                App.VM.ShowPopup(ex.Message, true);
            }
        }
    }
}
