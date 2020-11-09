using Lib.Share.Enums;
using Lib.Share.Models;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Resources;

namespace Clean_Reader.Models.UI
{
    public class CustomResourceLoader : CustomXamlResourceLoader
    {
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            if (resourceId == "Basic")
            {
                return new FontFamily(App.Tools.App.GetLocalSetting(SettingNames.FontFamily, StaticString.FontDefault));
            }
            else if (resourceId.Contains("Font"))
            {
                double NormalSize = Convert.ToDouble(App.Tools.App.GetLocalSetting(SettingNames.FontSize, "15"));
                if (resourceId == "BasicFontSize")
                    return NormalSize;
                else if (resourceId == "SmallFontSize")
                    return NormalSize * 0.85;
                else if (resourceId == "MiniFontSize")
                    return NormalSize * 0.7;
                else if (resourceId == "BodyFontSize")
                    return NormalSize * 1.2;
                else if (resourceId == "SubFontSize")
                    return NormalSize * 1.5;
                else if (resourceId == "HeaderFontSize")
                    return NormalSize * 2;
                else if (resourceId == "LargeFontSize")
                    return NormalSize * 2.5;
            }
            return null;
        }
    }
}
