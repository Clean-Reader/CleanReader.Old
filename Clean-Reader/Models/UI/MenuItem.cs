using Clean_Reader.Models.Enums;
using Richasy.Font.UWP.Enums;
using System.Collections.Generic;

namespace Clean_Reader.Models.UI
{
    public class MenuItem
    {
        public FeatherSymbol Icon { get; set; }
        public string Name { get; set; }
        public MenuItemType Type { get; set; }
        public MenuItem(MenuItemType type)
        {
            Type = type;
            Name = App.Tools.App.GetLocalizationTextFromResource(type);
            switch (type)
            {
                case MenuItemType.Shelf:
                    Icon = FeatherSymbol.BookOpen;
                    break;
                case MenuItemType.Discovery:
                    Icon = FeatherSymbol.Compass;
                    break;
                case MenuItemType.Category:
                    Icon = FeatherSymbol.Grid;
                    break;
                case MenuItemType.Topic:
                    Icon = FeatherSymbol.Bookmark;
                    break;
                case MenuItemType.Setting:
                    Icon = FeatherSymbol.Settings;
                    break;
                default:
                    break;
            }
        }

        public static List<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>
            {
                new MenuItem(MenuItemType.Shelf),
                new MenuItem(MenuItemType.Discovery),
                new MenuItem(MenuItemType.Category),
                new MenuItem(MenuItemType.Topic),
                new MenuItem(MenuItemType.Setting)
            };
        }

        public override bool Equals(object obj)
        {
            return obj is MenuItem item &&
                   Type == item.Type;
        }

        public override int GetHashCode()
        {
            return 2049151605 + Type.GetHashCode();
        }
    }
}
