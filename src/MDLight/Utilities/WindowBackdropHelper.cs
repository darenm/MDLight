using System;

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

using WinRT;

namespace MDLight.Utilities
{
    internal class WindowBackdropHelper<T> where T : Window
    {
        WindowsSystemDispatcherQueueHelper _wsdqHelper; // See below for implementation.
        ISystemBackdropControllerWithTargets _backdropController;
        SystemBackdropConfiguration _configurationSource;
        private T _appWindow;



        internal WindowBackdropHelper(T appWindow)
        {
            _appWindow = appWindow;
        }
        internal bool TrySetSystemBackdrop(BackdropType backdrop)
        {
            bool isBackdropSupported = backdrop switch
            {
                BackdropType.Acrylic => DesktopAcrylicController.IsSupported(),
                BackdropType.Mica => MicaController.IsSupported(),
                BackdropType.MicaAlt => MicaController.IsSupported(),
                _ => false
            };
            
            if (isBackdropSupported)
            {
                _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Create the policy object.
                _configurationSource = new SystemBackdropConfiguration();
                _appWindow.Activated += Window_Activated;
                _appWindow.Closed += Window_Closed;
                ((FrameworkElement)_appWindow.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                _configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                switch (backdrop)
                {
                    case BackdropType.Acrylic:
                        SetAcrylic();
                        break;
                    case BackdropType.Mica:
                        SetMica();
                        break;
                    case BackdropType.MicaAlt:
                        SetMicaWithAlt();
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected backdrop value");
                }

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                _backdropController.AddSystemBackdropTarget(_appWindow.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                _backdropController.SetSystemBackdropConfiguration(_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        private void SetAcrylic()
        {
            _backdropController = new DesktopAcrylicController();
        }

        private void SetMicaWithAlt()
        {
            _backdropController = new MicaController()
            {
                Kind = MicaKind.BaseAlt
            };
        }

        private void SetMica()
        {
            _backdropController = new MicaController();
        }
        
        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed
            // so it doesn't try to use this closed window.
            if (_backdropController != null)
            {
                _backdropController.Dispose();
                _backdropController = null;
            }
            _appWindow.Activated -= Window_Activated;
            _configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)_appWindow.Content).ActualTheme)
            {
                case ElementTheme.Dark: _configurationSource.Theme = SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: _configurationSource.Theme = SystemBackdropTheme.Light; break;
                case ElementTheme.Default: _configurationSource.Theme = SystemBackdropTheme.Default; break;
            }
        }

    }
}
