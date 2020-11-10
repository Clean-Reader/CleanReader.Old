using Clean_Reader.Controls.Layout;
using Richasy.Controls.Reader;
using Richasy.Helper.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuenov.SDK;

namespace Clean_Reader.Models.Core
{
    public partial class AppViewModel
    {
        public YuenovClient _yuenovClient;

        public ReaderPanel _reader;

        public SidePanel _sidePanel;

        private const string _clientId = "4ce94634-4e8d-4e7a-9967-18c59afd1dc7";
        private string[] _scopes= new string[] { "Files.ReadWrite.AppFolder", "User.Read" };
        public OneDriveHelper _onedrive;
    }
}
