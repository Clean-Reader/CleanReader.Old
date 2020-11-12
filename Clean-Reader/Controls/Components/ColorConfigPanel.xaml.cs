using Clean_Reader.Models.Core;
using Clean_Reader.Models.UI;
using Lib.Share.Enums;
using Lib.Share.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Clean_Reader.Controls.Components
{
    public sealed partial class ColorConfigPanel : UserControl
    {
        public bool IsInit = false;
        AppViewModel vm = App.VM;
        public ColorConfigPanel()
        {
            this.InitializeComponent();
        }
        public void Init()
        {
            IsInit = false;
            ForegroundColorPicker.Color = vm.ReaderStyle.Foreground;
            BackgroundColorPicker.Color = vm.ReaderStyle.Background;
            IsAcrylicBackground.IsChecked = vm.ReaderStyle.IsAcrylicBackground;
            IsInit = true;
        }
        private void ForegroundColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (!IsInit)
                return;
            vm.ChangeReaderColor(args.NewColor, Colors.Transparent);
        }

        private void BackgroundColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (!IsInit)
                return;
            vm.ChangeReaderColor(Colors.Transparent, args.NewColor);
        }

        private void IsAcrylicBackground_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInit)
                return;
            vm.ChangeReaderColor(Colors.Transparent, Colors.Transparent, IsAcrylicBackground.IsChecked);
        }

        private async void AddColorConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var foreground = ForegroundColorPicker.Color;
            var background = BackgroundColorPicker.Color;
            bool isAcrylic = Convert.ToBoolean(IsAcrylicBackground.IsChecked);
            var color = new ReaderColorConfig(foreground, background, isAcrylic);
            if (vm.ColorConfigCollection.Any(p => p.Equals(color)))
            {
                App.VM.ShowPopup(LanguageNames.RepeatColor, true);
            }
            else
            {
                vm.ColorConfigCollection.Insert(0, color);
                await App.Tools.IO.SetLocalDataAsync(StaticString.FileColorConfig, JsonConvert.SerializeObject(vm.ColorConfigCollection.ToList()));
            }
        }

        private void ColorDisplay_Apply(object sender, ColorEventArgs e)
        {
            ForegroundColorPicker.Color = e.Color.Foreground;
            BackgroundColorPicker.Color = e.Color.Background;
            IsAcrylicBackground.IsChecked = e.Color.IsAcrylicBackground;
            vm.ChangeReaderColor(e.Color.Foreground, e.Color.Background, e.Color.IsAcrylicBackground);
        }

        private async void ColorDisplay_Delete(object sender, ColorEventArgs e)
        {
            vm.ColorConfigCollection.Remove(e.Color);
            await App.Tools.IO.SetLocalDataAsync(StaticString.FileColorConfig, JsonConvert.SerializeObject(vm.ColorConfigCollection.ToList()));
        }
    }
}
