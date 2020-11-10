using Clean_Reader.Models.Enums;
using Lib.Share.Enums;
using Richasy.Controls.UWP.Models.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Clean_Reader.Models.UI
{
    public class EntryItem : NotifyPropertyBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        private int _count;
        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        public GroupType GroupType { get; set; }
        public EntryType EntryType { get; set; }

        public string Parameter { get; set; }


        public override bool Equals(object obj)
        {
            return obj is EntryItem item &&
                   Name == item.Name &&
                   GroupType == item.GroupType &&
                   EntryType == item.EntryType;
        }

        public override int GetHashCode()
        {
            int hashCode = -1668938187;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + GroupType.GetHashCode();
            hashCode = hashCode * -1521134295 + EntryType.GetHashCode();
            return hashCode;
        }

        public EntryItem()
        {

        }

        /// <summary>
        /// 书架条目
        /// </summary>
        /// <param name="name">书架名</param>
        /// <param name="id">书架ID</param>
        /// <param name="count">书架内包含的书本数</param>
        public EntryItem(string name,string id,int count)
        {
            Name = name;
            GroupType = GroupType.Shelf;
            EntryType = EntryType.Unspecific;
            Parameter = id;
            Count = count;
            IsSelected = false;
        }

        /// <summary>
        /// 书城条目
        /// </summary>
        /// <param name="name">分类名</param>
        public EntryItem(string name)
        {
            Name = name;
            GroupType = GroupType.Store;
            EntryType = EntryType.Unspecific;
            Parameter = "";
            Count = -1;
            IsSelected = false;
        }

        /// <summary>
        /// 发现条目
        /// </summary>
        /// <param name="type">条目类型</param>
        public EntryItem(EntryType type)
        {
            EntryType = type;
            GroupType = GroupType.Discovery;
            IsSelected = false;
            Count = -1;
            Parameter = "";
            Name = App.Tools.App.GetLocalizationTextFromResource(type);
        }

        public static List<EntryItem> GetDiscoveryList()
        {
            return new List<EntryItem>
            {
                new EntryItem(EntryType.Category),
                new EntryItem(EntryType.Rank),
                new EntryItem(EntryType.EndBooks),
                new EntryItem(EntryType.Topic),
            };
        }
    }
}
