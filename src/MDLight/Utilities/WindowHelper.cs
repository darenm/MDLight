using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace MDLight.Utilities;
public static partial class WindowHelper
{
    private static NativeMethods.WinProc newWndProc = null;
    private static IntPtr oldWndProc = IntPtr.Zero;

    /// <summary>
    /// Default is 900
    /// </summary>
    public static int MinWindowWidth { get; set; } = 900;
    /// <summary>
    /// Default is 1800
    /// </summary>
    public static int MaxWindowWidth { get; set; } = 1800;
    /// <summary>
    /// Default is 600
    /// </summary>
    public static int MinWindowHeight { get; set; } = 600;
    /// <summary>
    /// Default is 1600
    /// </summary>
    public static int MaxWindowHeight { get; set; } = 1600;

    public static void RegisterWindowMinMax(this Window window)
    {
        //Get the Window's HWND
        var hwnd = GetWindowHandleForCurrentWindow(window);

        newWndProc = new NativeMethods.WinProc(WndProc);
        oldWndProc = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
    }
    private static IntPtr WndProc(IntPtr hWnd, NativeMethods.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        switch (Msg)
        {
            case NativeMethods.WindowMessage.WM_GETMINMAXINFO:
                var dpi = NativeMethods.GetDpiForWindow(hWnd);
                var scalingFactor = (float)dpi / 96;

                var minMaxInfo = Marshal.PtrToStructure<NativeMethods.MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.x = (int)(MinWindowWidth * scalingFactor);
                minMaxInfo.ptMaxTrackSize.x = (int)(MaxWindowWidth * scalingFactor);
                minMaxInfo.ptMinTrackSize.y = (int)(MinWindowHeight * scalingFactor);
                minMaxInfo.ptMaxTrackSize.y = (int)(MaxWindowHeight * scalingFactor);

                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;

        }
        return NativeMethods.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
    }

    /// <summary>
    /// Set Window Width and Height
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public static void SetWindowSize(this Window window, int width, int height)
    {
        var hwnd = GetWindowHandleForCurrentWindow(window);
        // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
        var dpi = NativeMethods.GetDpiForWindow(hwnd);
        var scalingFactor = (float)dpi / 96;
        width = (int)(width * scalingFactor);
        height = (int)(height * scalingFactor);

        NativeMethods.SetWindowPos(hwnd, NativeMethods.HWND_TOP, 0, 0, width, height, NativeMethods.SetWindowPosFlags.SWP_NOMOVE);
    }

    /// <summary>
    /// allow the app to find the Window that contains an
    /// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
    /// of all active Windows.  The app code must call WindowHelper.CreateWindow
    /// rather than "new Window" so we can keep track of all the relevant windows.
    /// </summary>
    public static List<Window> ActiveWindows { get { return _activeWindows; } }

    private static List<Window> _activeWindows = new();

    /// <summary>
    /// Get AppWindow For a Window
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static AppWindow GetAppWindowForCurrentWindow(object target)
    {
        return AppWindow.GetFromWindowId(GetWindowIdFromCurrentWindow(target));
    }

    /// <summary>
    /// Get WindowHandle for a Window
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static IntPtr GetWindowHandleForCurrentWindow(object target)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(target);
        return hWnd;
    }

    /// <summary>
    /// Get WindowId from Window
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static WindowId GetWindowIdFromCurrentWindow(object target)
    {
        var wndId = Win32Interop.GetWindowIdFromWindow(GetWindowHandleForCurrentWindow(target));
        return wndId;
    }

    /// <summary>
    /// allow the app to find the Window that contains an
    /// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
    /// of all active Windows.  The app code must call WindowHelper.CreateWindow
    /// rather than "new Window" so we can keep track of all the relevant windows.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Window GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (var window in _activeWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Create a new Window
    /// </summary>
    /// <returns></returns>
    public static Window CreateWindow()
    {
        var newWindow = new Window();
        TrackWindow(newWindow);
        return newWindow;
    }

    /// <summary>
    /// track of all active Windows.  The app code must call WindowHelper.CreateWindow
    /// rather than "new Window" so we can keep track of all the relevant windows.
    /// </summary>
    /// <param name="window"></param>
    public static void TrackWindow(Window window)
    {
        window.Closed += (sender, args) => {
            _activeWindows.Remove(window);
        };
        _activeWindows.Add(window);
    }

    public static void SwitchToThisWindow(object target)
    {
        if (target != null)
        {
            NativeMethods.SwitchToThisWindow(GetWindowHandleForCurrentWindow(target), true);
        }
    }
}
