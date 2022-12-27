using System;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace MDLight.Services
{
    public class WindowsService : IWindowsService
    {
        public IntPtr HWnd { get; set; }
        public AppWindow AppWindow { get; set; }
        public Window MainWindow { get; set; }
    }
}
