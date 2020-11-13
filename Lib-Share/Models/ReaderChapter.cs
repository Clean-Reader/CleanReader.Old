using Richasy.Controls.Reader.Models;

namespace Lib.Share.Models
{
    public class ReaderChapter
    {
        public string Content { get; set; }
        public Chapter Chapter { get; set; }

        public static Chapter GetChapterFromWeb(int index, Yuenov.SDK.Models.Share.Chapter chapter)
        {
            var c = new Chapter();
            c.Hash = "";
            c.HashIndex = 0;
            c.Index = index;
            c.Level = 1;
            c.Link = chapter.Id.ToString();
            c.Title = chapter.Name;
            c.StartLength = 0;
            return c;
        }
    }
}
