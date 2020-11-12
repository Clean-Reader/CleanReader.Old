using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Clean_Reader.Models.UI
{
    public static class StaticUIExtension
    {
        /// <summary>
        /// 16进制转RGB
        /// </summary>
        /// <param name="strHxColor">16进制颜色</param>
        /// <param name="opacity">不透明度，0-1之间</param>
        /// <returns>转换后的<see cref="Color"/></returns>
        public static Color Hex16toRGB(this string strHxColor, double opacity = 1)
        {
            if (opacity < 0 || opacity > 1) { throw new ArgumentOutOfRangeException("Opacity"); }
            try
            {
                byte a, r, g, b;
                if (strHxColor.Length > 7)
                {
                    a = byte.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    r = byte.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    g = byte.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    b = byte.Parse(strHxColor.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    a = byte.Parse((Convert.ToInt32(opacity * 255)).ToString(), System.Globalization.NumberStyles.Integer);
                    r = byte.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    g = byte.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    b = byte.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                }

                if (strHxColor.Length == 0)
                {//如果为空
                    return Color.FromArgb(255, 0, 0, 0);//设为黑色
                }
                else
                {//转换颜色
                    var color = Color.FromArgb(a, r, g, b);
                    return color;
                }
            }
            catch
            {//设为黑色
                return Color.FromArgb(255, 0, 0, 0);
            }
        }
    }
}
