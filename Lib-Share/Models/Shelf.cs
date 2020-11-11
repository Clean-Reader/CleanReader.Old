using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;

namespace Lib.Share.Models
{
    public class Shelf : NotifyPropertyBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Shelf shelf &&
                   Id == shelf.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }

        public Shelf() { }

        public Shelf(string name, string id = "")
        {
            if (string.IsNullOrEmpty(id))
                Id = Guid.NewGuid().ToString("N");
            else
                Id = id;
            Name = name;
        }
    }
}
