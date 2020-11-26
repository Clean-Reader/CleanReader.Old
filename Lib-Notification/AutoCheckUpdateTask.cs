using Lib.Share.Enums;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.Reader.Models;
using Richasy.Helper.UWP;
using Richasy.Helper.UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Yuenov.SDK.Enums;
using Yuenov.SDK.Models.Shelf;

namespace Lib.Notification
{
    public sealed class AutoCheckUpdateTask:IBackgroundTask
    {
        private List<Book> TotalBookList = null;
        private Yuenov.SDK.YuenovClient _yuenovClient = new Yuenov.SDK.YuenovClient();
        private Instance _instance = new Instance(StaticString.AppName);
        private bool IsBookListChanged = false;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var def = taskInstance.GetDeferral();
            _yuenovClient.SetOpenToken("e89309f4-6cd8-4a45-90de-922e7d71455a");
            TotalBookList = await _instance.IO.GetLocalDataAsync<List<Book>>(StaticString.FileShelfList);
            var webBooks = TotalBookList.Where(p => p.Type == BookType.Web).ToList();
            if (webBooks.Count > 0)
            {
                await UpdateBooks(webBooks.ToArray());
                if (IsBookListChanged)
                {
                    await _instance.IO.SetLocalDataAsync(StaticString.FileShelfList, JsonConvert.SerializeObject(TotalBookList));
                }
            }
            _instance.App.WriteLocalSetting(SettingNames.LastBackgroundSyncTime, _instance.App.GetNowSeconds().ToString());
            def.Complete();
        }
        private Chapter GetChapterFromWeb(int index, Yuenov.SDK.Models.Share.Chapter chapter)
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
        private async Task<List<Chapter>> SyncBookChapters(int bookId, long startChapterId = 0)
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
                        var sourceList = await _instance.IO.GetLocalDataAsync<List<Chapter>>(bookId + ".json", "[]", StaticString.FolderChapter);
                        int lastIndex = sourceList.Last().Index;
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].Index += lastIndex;
                            if (!sourceList.Any(predicate => predicate.Link == result[i].Link))
                                sourceList.Add(result[i]);
                        }
                        result = sourceList;
                    }
                    if (sourceBook != null)
                    {
                        sourceBook.LastChapterId = chapters.Last().Id;
                        IsBookListChanged = true;
                    }
                    await _instance.IO.SetLocalDataAsync(bookId + ".json", JsonConvert.SerializeObject(result), StaticString.FolderChapter);
                }
            }
            catch (Exception)
            {}
            return result;
        }

        private async Task UpdateBooks(params Book[] books)
        {
            var items = new List<CheckUpdateItem>();
            foreach (var book in books)
            {
                items.Add(new CheckUpdateItem(Convert.ToInt32(book.BookId), book.LastChapterId));
            }
            var response = await _yuenovClient.CheckUpdateAsync(items.ToArray());
            if (response.Result.Code == ResultCode.Success)
            {
                if (response.Data.UpdateList.Count > 0)
                {
                    var tasks = new List<Task>();
                    foreach (var up in response.Data.UpdateList)
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
        }
    }
}
