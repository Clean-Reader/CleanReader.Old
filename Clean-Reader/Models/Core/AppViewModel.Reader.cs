using Clean_Reader.Models.UI;
using Lib.Share.Models;
using Richasy.Controls.Reader.Models;
using Richasy.Font.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

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
            ReaderStyle.HeaderMargin = new Thickness(0, 0, 0, size);
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
    }
}
