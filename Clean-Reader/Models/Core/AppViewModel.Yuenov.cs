using Clean_Reader.Models.UI;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.Reader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuenov.SDK.Enums;
using Yuenov.SDK.Models.Shelf;

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

        public Chapter GetChapterFromWeb(int index, Yuenov.SDK.Models.Share.Chapter chapter)
        {
            var c = new Chapter();
            c.Hash = "";
            c.HashIndex = 0;
            c.Index = index;
            c.Level = 0;
            c.Link = chapter.Id.ToString();
            c.Title = chapter.Name;
            c.StartLength = 0;
            return c;
        }

        /// <summary>
        /// 同步书籍目录信息
        /// </summary>
        /// <param name="bookId">书籍ID</param>
        /// <param name="startChapterId">起始章节ID</param>
        /// <returns></returns>
        public async Task<List<Chapter>> SyncBookChapters(int bookId, long startChapterId = 0)
        {
            var result = new List<Chapter>();
            try
            {
                var sourceBook = TotalBookList.Where(p => p.BookId == bookId.ToString()).FirstOrDefault();
                var response = await _yuenovClient.GetBookChaptersAsync(bookId, startChapterId);
                if (response.Result.Code == ResultCode.Success)
                {
                    var chapters = response.Data.Chapters;
                    for (int i = 0; i < chapters.Count; i++)
                    {
                        result.Add(GetChapterFromWeb(i + 1, chapters[i]));
                    }
                    if (startChapterId > 0)
                    {
                        var sourceList = await App.Tools.IO.GetLocalDataAsync<List<Chapter>>(bookId + ".json", "[]", StaticString.FolderChapter);
                        int lastIndex = sourceList.Last().Index;
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].Index += lastIndex;
                            sourceList.Add(result[i]);
                        }
                        result = sourceList;
                    }
                    if (sourceBook != null)
                    {
                        sourceBook.LastChapterId = chapters.Last().Id;
                        IsShelfChanged = true;
                    }   
                    await App.Tools.IO.SetLocalDataAsync(bookId + ".json", JsonConvert.SerializeObject(result), StaticString.FolderChapter);
                    return result;
                }
                else
                    App.VM.ShowPopup($"{response.Result.Code}: {response.Result.Message}");
            }
            catch (Exception ex)
            {
                App.VM.ShowPopup(ex.Message, true);
            }
            return result;
        }

        public async Task UpdateBooks(params Book[] books)
        {
            var items = new List<CheckUpdateItem>();
            foreach (var book in books)
            {
                items.Add(new CheckUpdateItem(Convert.ToInt32(book.BookId), book.LastChapterId));
            }
            var response = await _yuenovClient.CheckUpdateAsync(items.ToArray());
            if (response.Result.Code == ResultCode.Success)
            {
                if (response.Data.Count > 0)
                {
                    var tasks = new List<Task>();
                    foreach (var up in response.Data)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            var source = books.Where(p => p.BookId == up.BookId.ToString()).FirstOrDefault();
                            await SyncBookChapters(up.BookId, source.LastChapterId);
                        }));
                    }
                    await Task.WhenAll(tasks.ToArray());
                }
            }
            else
                App.VM.ShowPopup($"{response.Result.Code}: {response.Result.Message}");
        }
    }
}
