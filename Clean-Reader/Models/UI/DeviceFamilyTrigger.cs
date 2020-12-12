using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace Clean_Reader.Models.UI
{
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private string _currentDeviceFamily, _queriedDeviceFamily;

        public string DeviceFamily
        {
            get
            {
                return _queriedDeviceFamily;
            }

            set
            {
                _queriedDeviceFamily = value;
                _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
                SetActive(_queriedDeviceFamily == _currentDeviceFamily);
            }
        }
    }
}
