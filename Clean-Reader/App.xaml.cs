using Richasy.Helper.UWP;
using Clean_Reader.Models.Core;
using Clean_Reader.Models.UI;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;
using Lib.Share.Models;
using Lib.Share.Enums;
using System.Text;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Storage;
using Clean_Reader.Controls.Dialogs;

namespace Clean_Reader
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public static AppViewModel VM = new AppViewModel();
        public static Instance Tools = new Instance(StaticString.AppName);
        public App()
        {
            this.InitializeComponent();
            bool isDisableScale = Tools.App.GetBoolSetting(SettingNames.DisableXboxScale);
            if (SystemInformation.DeviceFamily == "Windows.Xbox" && isDisableScale)
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
                Windows.UI.ViewManagement.ApplicationViewScaling.TrySetDisableLayoutScaling(true);
            }
            VM.LanguageInit();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
            string theme = Tools.App.GetLocalSetting(SettingNames.Theme, StaticString.ThemeSystem);
            if (theme != StaticString.ThemeSystem)
                RequestedTheme = theme == StaticString.ThemeLight ? ApplicationTheme.Light : ApplicationTheme.Dark;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CustomXamlResourceLoader.Current = new CustomResourceLoader();
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            VM.ShowPopup(e.Message, true);
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            OnLaunchedOrActivated(args);
        }
        protected override void OnActivated(IActivatedEventArgs args)
        {
            OnLaunchedOrActivated(args);
        }
        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            OnLaunchedOrActivated(e);
        }
        private async void OnLaunchedOrActivated(IActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            Tools = new Instance(StaticString.AppName);
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e is LaunchActivatedEventArgs && (e as LaunchActivatedEventArgs).PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), (e as LaunchActivatedEventArgs).Arguments);
                }
            }
            else if (e is ProtocolActivatedEventArgs protocalArgs)
            {
                string arg = protocalArgs.Uri.Query.Replace("?", "");
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), arg);
                }
            }
            else if(e is FileActivatedEventArgs fileArgs)
            {
                var file = fileArgs.Files[0];
                if (file is StorageFile sf)
                {
                    if (rootFrame.Content == null)
                    {
                        rootFrame.Navigate(typeof(MainPage), file);
                    }
                    else
                    {
                        var dialog = new ConfirmDialog(LanguageNames.OpenFileWarning);
                        dialog.PrimaryButtonClick += async(_s, _e) =>
                        {
                            var book = await VM.ImportBook(sf);
                            VM.OpenReaderView(book);
                        };
                        await dialog.ShowAsync();
                    }
                }
            }
            else if (e.Kind == ActivationKind.StartupTask)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage));
                }
            }
            else if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage));
                }
                // Parse toastActivationArgs.Argument

            }
            Window.Current.Activate();
            Tools.App.SetTitleBarColor();
        }
        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
