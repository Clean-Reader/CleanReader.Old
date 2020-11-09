using Richasy.Controls.UWP.Popups;
using Richasy.Controls.UWP.Widgets;
using Richasy.Font.UWP;
using Richasy.Font.UWP.Enums;
using System;
using Windows.Globalization;
using Windows.Storage;
using Lib.Share.Enums;
using Lib.Share.Models;
using Yuenov.SDK;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public AppViewModel()
        {
            _yuenovClient = new YuenovClient();
        }
        
    }
}
