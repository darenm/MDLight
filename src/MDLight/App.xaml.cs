// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;

using MDLight.Services;
using MDLight.Utilities;
using MDLight.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDLight
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    services
                    .AddSingleton<IWindowsService, WindowsService>()
                    .AddSingleton<INavigationService, NavigationService>()
                    .AddSingleton<MainViewModel>())
                 .Build();

            ServicesResolver.Services = _host.Services;

            SystemTheme = RequestedTheme;
            var appTheme = SettingsHelper.GetSetting(AppSettings.AppTheme);
            if (appTheme != null)
            {
                switch (appTheme)
                {
                    case "Light":
                        RequestedTheme = ApplicationTheme.Light;
                        break;
                    case "Dark":
                        RequestedTheme = ApplicationTheme.Dark;
                        break;
                    default:
                        IsSystemTheme = true;
                        break;
                }
            }
            else
            {
                IsSystemTheme = true;
            }

        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            var windowService = ServicesResolver.Services.GetService<IWindowsService>();
            windowService.AppWindow = WindowHelper.GetAppWindowForCurrentWindow(m_window);
            windowService.MainWindow = m_window;
            windowService.HWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
        }

        private Window m_window;
        private IHost _host;

        public ApplicationTheme SystemTheme { get; }
        public bool IsSystemTheme { get; set; }
    }
}
