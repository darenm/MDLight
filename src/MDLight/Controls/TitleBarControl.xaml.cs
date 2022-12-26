// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI;
using System;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using WinRT.Interop;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDLight.Controls;


public sealed partial class TitleBarControl : UserControl
{
    private MainWindow _mainWindow;
    private AppWindow _appWindow;

    public TitleBarControl()
    {
        this.InitializeComponent();
    }

    internal void SetupTitlebar(
        string appTitle,
        AppWindow appWindow,
        MainWindow mainWindow,
        Func<bool> fallbackSetup)
    {
        _appWindow = appWindow;
        _mainWindow = mainWindow;
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            TitleTextBlock.Text = appTitle;
            var titleBar = _appWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;
            AppTitleBar.Loaded += AppTitleBar_Loaded;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
        }
        else
        {
            // Title bar customization using these APIs is currently
            // supported only on Windows 11. In other cases, hide
            // the custom title bar element.
            AppTitleBar.Visibility = Visibility.Collapsed;

            // Show alternative UI for any functionality in
            // the title bar, such as search.
            fallbackSetup();
        }
    }

    private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            SetDragRegionForCustomTitleBar(_appWindow);
        }
    }

    private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (AppWindowTitleBar.IsCustomizationSupported()
            && _appWindow.TitleBar.ExtendsContentIntoTitleBar)
        {
            // Update drag region if the size of the title bar changes.
            SetDragRegionForCustomTitleBar(_appWindow);
        }
    }

    private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
    {
        if (AppWindowTitleBar.IsCustomizationSupported()
            && appWindow.TitleBar.ExtendsContentIntoTitleBar)
        {
            double scaleAdjustment = GetScaleAdjustment();

            RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

            List<Windows.Graphics.RectInt32> dragRectsList = new();

            Windows.Graphics.RectInt32 dragRectL;
            dragRectL.X = (int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
            dragRectL.Y = 0;
            dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
            dragRectL.Width = (int)((IconColumn.ActualWidth
                                    + TitleColumn.ActualWidth
                                    + LeftDragColumn.ActualWidth) * scaleAdjustment);
            dragRectsList.Add(dragRectL);

            Windows.Graphics.RectInt32 dragRectR;
            dragRectR.X = (int)((LeftPaddingColumn.ActualWidth
                                + IconColumn.ActualWidth
                                + TitleTextBlock.ActualWidth
                                + LeftDragColumn.ActualWidth
                                + SearchColumn.ActualWidth) * scaleAdjustment);
            dragRectR.Y = 0;
            dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
            dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
            dragRectsList.Add(dragRectR);

            Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

            appWindow.TitleBar.SetDragRectangles(dragRects);
        }
    }

    private double GetScaleAdjustment()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(_mainWindow);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        // Get DPI.
        int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
        if (result != 0)
        {
            throw new Exception("Could not get DPI for monitor.");
        }

        uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
        return scaleFactorPercent / 100.0;
    }

    [DllImport("Shcore.dll", SetLastError = true)]
    internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

    internal enum Monitor_DPI_Type : int
    {
        MDT_Effective_DPI = 0,
        MDT_Angular_DPI = 1,
        MDT_Raw_DPI = 2,
        MDT_Default = MDT_Effective_DPI
    }

    private void SwitchPresenter(object sender, RoutedEventArgs e)
    {
        if (_appWindow != null)
        {
            AppWindowPresenterKind newPresenterKind;
            switch ((sender as Button).Name)
            {
                case "CompactoverlaytBtn":
                    newPresenterKind = AppWindowPresenterKind.CompactOverlay;
                    break;

                case "FullscreenBtn":
                    newPresenterKind = AppWindowPresenterKind.FullScreen;
                    break;

                case "OverlappedBtn":
                    newPresenterKind = AppWindowPresenterKind.Overlapped;
                    break;

                default:
                    newPresenterKind = AppWindowPresenterKind.Default;
                    break;
            }

            // If the same presenter button was pressed as the
            // mode we're in, toggle the window back to Default.
            if (newPresenterKind == _appWindow.Presenter.Kind)
            {
                _appWindow.SetPresenter(AppWindowPresenterKind.Default);
            }
            else
            {
                // Else request a presenter of the selected kind
                // to be created and applied to the window.
                _appWindow.SetPresenter(newPresenterKind);
            }
        }
    }
}
