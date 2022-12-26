// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MDLight.Utilities;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDLight
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        private WindowBackdropHelper<MainWindow> _backDropHelper;

        public MainWindow()
        {
            InitializeComponent();
            SetupWindow();
            SetupTitle();
            SetupBackdrop();
        }


        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //myButton.Content = "Clicked";
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {

        }

        private void OnElementClicked(object sender, RoutedEventArgs e)
        {

        }

        #region Setup   

        private void SetupBackdrop()
        {
            _backDropHelper = new WindowBackdropHelper<MainWindow>(this);
            _backDropHelper.TrySetSystemBackdrop(BackdropType.MicaAlt);
        }

        private void SetupTitle()
        {
            TitleBar.SetupTitlebar("MDLight", _appWindow, this, SetTitleBarColors);
        }

        private void SetupWindow()
        {
            _appWindow = WindowHelper.GetAppWindowForCurrentWindow(this);
            _appWindow.Title = "MDLight";
            WindowHelper.RegisterWindowMinMax(this);
            WindowHelper.MinWindowWidth = 400;
            WindowHelper.MinWindowHeight = 400;
            WindowHelper.SetWindowSize(this, 900, 600);
        }

        private bool SetTitleBarColors()
        {
            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                if (_appWindow is null)
                {
                    _appWindow = WindowHelper.GetAppWindowForCurrentWindow(this);
                }
                var titleBar = _appWindow.TitleBar;

                // Set active window colors
                titleBar.ForegroundColor = Colors.White;
                titleBar.BackgroundColor = Colors.Green;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonBackgroundColor = Colors.SeaGreen;
                titleBar.ButtonHoverForegroundColor = Colors.Gainsboro;
                titleBar.ButtonHoverBackgroundColor = Colors.DarkSeaGreen;
                titleBar.ButtonPressedForegroundColor = Colors.Gray;
                titleBar.ButtonPressedBackgroundColor = Colors.LightGreen;

                // Set inactive window colors
                titleBar.InactiveForegroundColor = Colors.Gainsboro;
                titleBar.InactiveBackgroundColor = Colors.SeaGreen;
                titleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
                titleBar.ButtonInactiveBackgroundColor = Colors.SeaGreen;
                return true;
            }
            return false;
        }

        #endregion Setup

    }
}


