using System;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace MDLight.Services
{
    public interface IWindowsService
    {
        AppWindow AppWindow { get; set; }
        Window MainWindow { get; set; }
        IntPtr HWnd { get; set; }
    }
}