using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Clean_Reader.Models.UI
{
    public class ReaderColorConfig : NotifyPropertyBase
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public bool IsAcrylicBackground { get; set; }
        public ReaderColorConfig() { }
        public ReaderColorConfig(Color foreground, Color background, bool isAcrylic = false)
        {
            Foreground = foreground;
            Background = background;
            IsAcrylicBackground = isAcrylic;
        }
        public static List<ReaderColorConfig> GetDefaultColors()
        {
            var list = new List<ReaderColorConfig>
            {
                new ReaderColorConfig(Colors.Black,"#FFFFFF".Hex16toRGB(0.6),true),
                new ReaderColorConfig("#D6D6D6".Hex16toRGB(),"#080707".Hex16toRGB(0.8),true),
                new ReaderColorConfig(Colors.Black,Colors.White),
                new ReaderColorConfig(Colors.Gray,"#0F0F0F".Hex16toRGB()),
                new ReaderColorConfig("#171717".Hex16toRGB(),"#C7EDCC".Hex16toRGB()),
                new ReaderColorConfig("#171717".Hex16toRGB(),"#F1E5C9".Hex16toRGB()),
                new ReaderColorConfig("#1a2430".Hex16toRGB(),"#f2f2f4".Hex16toRGB()),
                new ReaderColorConfig("#0d1f45".Hex16toRGB(),"#fdd3e7".Hex16toRGB()),
                new ReaderColorConfig("#121212".Hex16toRGB(),"#A2AEC9".Hex16toRGB())
            };
            return list;
        }

        public override bool Equals(object obj)
        {
            return obj is ReaderColorConfig color &&
                   EqualityComparer<Color>.Default.Equals(Foreground, color.Foreground) &&
                   EqualityComparer<Color>.Default.Equals(Background, color.Background) &&
                   IsAcrylicBackground == color.IsAcrylicBackground;
        }

        public override int GetHashCode()
        {
            var hashCode = -1683514879;
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Foreground);
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Background);
            hashCode = hashCode * -1521134295 + IsAcrylicBackground.GetHashCode();
            return hashCode;
        }
    }
}
