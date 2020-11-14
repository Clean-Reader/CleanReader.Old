using Clean_Reader.Models.UI;
using Clean_Reader.Pages;
using Lib.Share.Models;
using Newtonsoft.Json;
using Richasy.Controls.Reader.Models;
using Richasy.Font.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public async void ViewStyleInit()
        {
            var readerStyle = await App.Tools.IO.GetLocalDataAsync<ReaderStyle>(StaticString.FileReaderStyle, "{}");
            ReaderStyle = readerStyle == null ? new ReaderStyle() : readerStyle;
        }
        public async void ColorConfigInit()
        {
            ColorConfigCollection.Clear();
            var colorConfigs = await App.Tools.IO.GetLocalDataAsync<List<ReaderColorConfig>>(StaticString.FileColorConfig);
            if (colorConfigs.Count == 0)
                colorConfigs = ReaderColorConfig.GetDefaultColors();
            colorConfigs.ForEach(p => ColorConfigCollection.Add(p));
        }
        public void ChangeReaderFont(SystemFont font)
        {
            ReaderStyle.FontFamily = font.Name;
            UpdateStyle();
        }

        public void ChangeReaderFontSize(double size)
        {
            ReaderStyle.FontSize = size;
            ReaderStyle.HeaderFontSize = size * 1.6;
            ReaderStyle.HeaderMargin = new Thickness(0, 0, 0, size * 2);
            UpdateStyle();
        }

        public void ChangeReaderColor(Color fore, Color back, bool? isAcrylic = null)
        {
            if (fore != Colors.Transparent)
                ReaderStyle.Foreground = fore;
            if (back != Colors.Transparent)
                ReaderStyle.Background = back;
            if (isAcrylic != null)
                ReaderStyle.IsAcrylicBackground = Convert.ToBoolean(isAcrylic);
            UpdateStyle();
        }

        public void UpdateStyle()
        {
            _reader.UpdateStyle(ReaderStyle);
            _isStyleChanged = true;
        }

        public Brush GetBackgroundBrush()
        {
            if (ReaderStyle.IsAcrylicBackground)
            {
                var opacity = Convert.ToInt32(ReaderStyle.Background.A) / 255.0;
                var tempBackground = ReaderStyle.Background;
                tempBackground.A = 255;
                var acrylic = new AcrylicBrush()
                {
                    TintColor = tempBackground,
                    TintOpacity = opacity,
                    FallbackColor = ReaderStyle.Background,
                    BackgroundSource = AcrylicBackgroundSource.HostBackdrop
                };
                return acrylic;
            }
            else
                return new SolidColorBrush(ReaderStyle.Background);
        }

        public async Task<List<Chapter>> GetBookLocalChapters(string bookId, bool setCurrent = false)
        {
            try
            {
                var localRecord = await App.Tools.IO.GetLocalDataAsync<List<Chapter>>(bookId + ".json", folderName: StaticString.FolderChapter);
                if (setCurrent)
                    CurrentBookChapterList = localRecord;
                return localRecord;
            }
            catch (Exception)
            {
                return new List<Chapter>();
            }
        }

        public async Task SetBookLocalChapters(string bookId, List<Chapter> chapters, bool setCurrent = false)
        {
            if (setCurrent)
                CurrentBookChapterList = chapters;
            await App.Tools.IO.SetLocalDataAsync(bookId + ".json", JsonConvert.SerializeObject(chapters), StaticString.FolderChapter);
        }

        public async Task<List<ChapterDetail>> GetBookLocalChapterDetails(string bookId, bool setCurrent = false)
        {
            try
            {
                var localRecord = await App.Tools.IO.GetLocalDataAsync<List<ChapterDetail>>(bookId + ".json", folderName: StaticString.FolderChapterDetail);
                if (setCurrent)
                    CurrentBookChapterDetailList = localRecord;
                return localRecord;
            }
            catch (Exception)
            {
                return new List<ChapterDetail>();
            }
        }

        public async Task<ChapterDetail> RequestChapterDetail(string bookId, Chapter target)
        {
            var result = CurrentBookChapterDetailList.Where(p => p.Index == target.Index).FirstOrDefault();
            if (result != null)
                return result;
            try
            {
                var response = await _yuenovClient.DownloadChaptersAsync(Convert.ToInt32(bookId), Convert.ToInt64(target.Link));
                if (response.Result.Code == Yuenov.SDK.Enums.ResultCode.Success)
                {
                    var detail = response.Data.List.First();
                    var item = new ChapterDetail(target.Index, target.Title, detail.Content);
                    CurrentBookChapterDetailList.Add(item);
                    IsDetailChanged = true;
                    return item;
                }
                else
                    ShowPopup($"{response.Result.Code}: {response.Result.Message}");
            }
            catch (Exception ex)
            {
                ShowPopup(ex.Message, true);
            }
            return null;
        }
    }
}
