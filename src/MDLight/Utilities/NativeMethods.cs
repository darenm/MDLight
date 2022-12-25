﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MDLight.Utilities;
internal static class NativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern void SwitchToThisWindow(IntPtr hWnd, bool turnOn);

    internal delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("NTdll.dll")]
    internal static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTL_OSVERSIONINFOEX
    {
        public RTL_OSVERSIONINFOEX(uint dwMajorVersion, uint dwMinorVersion, uint dwBuildNumber, uint dwRevision, uint dwPlatformId, string szCSDVersion) : this()
        {
            this.dwMajorVersion = dwMajorVersion;
            this.dwMinorVersion = dwMinorVersion;
            this.dwBuildNumber = dwBuildNumber;
            this.dwRevision = dwRevision;
            this.dwPlatformId = dwPlatformId;
            this.szCSDVersion = szCSDVersion;
        }
        public readonly uint dwOSVersionInfoSize { get; init; } = (uint)Marshal.SizeOf<RTL_OSVERSIONINFOEX>();

        public uint dwMajorVersion;
        public uint dwMinorVersion;
        public uint dwBuildNumber;
        public uint dwRevision;
        public uint dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
    }

    /// <summary>
    /// Places the window at the top of the Z order.
    /// </summary>
    internal static readonly IntPtr HWND_TOP = new(0);

    [DllImport("CoreMessaging.dll")]
    internal static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

    [DllImport("User32.dll")]
    internal static extern int GetDpiForWindow(IntPtr hwnd);

    [DllImport("User32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [DllImport("Shcore.dll", SetLastError = true)]
    internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

    [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GetCurrentPackageFullName(ref uint packageFullNameLength, [Optional] StringBuilder packageFullName);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    internal static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    internal static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
    {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, newProc);
        else
            return new IntPtr(SetWindowLong32(hWnd, nIndex, newProc));
    }

    [DllImport("user32.dll")]
    internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// An attribute applied to native pointer parameters to indicate its semantics
    /// such that a friendly overload of the method can be generated with the right signature.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Conditional("CodeGeneration")]
    public class FriendlyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendlyAttribute"/> class.
        /// </summary>
        /// <param name="flags">The flags that describe this parameter's semantics.</param>
        public FriendlyAttribute(FriendlyFlags flags)
        {
            Flags = flags;
        }

        /// <summary>
        /// Gets the flags that describe this parameter's semantics.
        /// </summary>
        public FriendlyFlags Flags { get; }

        /// <summary>
        /// Gets or sets the 0-based index to the parameter that takes the length of the array given by this array parameter.
        /// An overload will be generated that drops this parameter and sets it automatically based on the length of the array provided to the parameter this attribute is applied to.
        /// </summary>
        /// <value>-1 is the default and indicates that no automated parameter removal should be generated.</value>
        /// <remarks>
        /// Only applicable when <see cref="Flags"/> includes <see cref="FriendlyFlags.Array"/>.
        /// </remarks>
        public int ArrayLengthParameter { get; set; } = -1;
    }

    [Flags]
    public enum FriendlyFlags
    {
        /// <summary>
        /// The pointer is to the first element in an array.
        /// </summary>
        Array = 0x1,

        /// <summary>
        /// The value is at least partially initialized by the caller.
        /// </summary>
        In = 0x2
    }

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        /// <summary>
        ///     Retains the current position (ignores X and Y parameters).
        /// </summary>
        SWP_NOMOVE = 0x0002
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DispatcherQueueOptions
    {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }

    internal enum Monitor_DPI_Type : int
    {
        MDT_Effective_DPI = 0,
        MDT_Angular_DPI = 1,
        MDT_Raw_DPI = 2,
        MDT_Default = MDT_Effective_DPI
    }

    [Flags]
    internal enum WindowLongIndexFlags : int
    {
        GWL_WNDPROC = -4,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    public enum WindowMessage : int
    {
        WM_GETMINMAXINFO = 0x0024,
    }

    public struct POINT
    {
        /// <summary>
        ///  The x-coordinate of the point.
        /// </summary>
        public int x;

        /// <summary>
        /// The x-coordinate of the point.
        /// </summary>
        public int y;

#if !UAP10_0
        public static implicit operator System.Drawing.Point(POINT point) => new(point.x, point.y);

        public static implicit operator POINT(System.Drawing.Point point) => new() { x = point.X, y = point.Y };
#endif
    }
}