
using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using MDLight.Messages;
using MDLight.Models;
using MDLight.Services;
using MDLight.Utilities;
using MDLight.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace MDLight.Controls
{
    public sealed partial class MainPage : Page
    {
        private IWindowsService _windowsService;
        private INavigationService _navigationService;
        private List<MarkdownDocument> _documents = new List<MarkdownDocument>();
        private Brush _orangeBrush = new SolidColorBrush(Colors.Orange);

        public event EventHandler<ElementTheme> ThemeChanged;

        public bool ShowSettings
        {
            get { return (bool)GetValue(ShowSettingsProperty); }
            set { SetValue(ShowSettingsProperty, value); }
        }

        public static readonly DependencyProperty ShowSettingsProperty =
            DependencyProperty.Register("ShowSettings", typeof(bool), typeof(MainPage), new PropertyMetadata(false));


        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainPage), new PropertyMetadata(null));


        public MainPage()
        {
            this.InitializeComponent();

            DataContext = this;
            ViewModel = ServicesResolver.Services.GetService<MainViewModel>();
            _windowsService = ServicesResolver.Services.GetService<IWindowsService>();
            _navigationService = ServicesResolver.Services.GetService<INavigationService>();

            // Register a message in some module
            WeakReferenceMessenger.Default.Register<ShowSettingsMessage>(this, (r, m) =>
            {
                // Handle the message here, with r being the recipient and m being the
                // input message. Using the recipient passed as input makes it so that
                // the lambda expression doesn't capture "this", improving performance.
                OnSettingsClicked();
            });
        }


        private void OnSettingsClicked()
        {
            _navigationService.CanGoBack = true;
            ShowSettings = true;

            if (((App)Application.Current).IsSystemTheme)
            {
                System.IsChecked = true;
            }
            else
            {
                if (RootGrid.RequestedTheme == ElementTheme.Default)
                {
                    switch (((App)Application.Current).RequestedTheme)
                    {
                        case ApplicationTheme.Light:
                            Light.IsChecked = true;
                            break;
                        case ApplicationTheme.Dark:
                            Dark.IsChecked = true;
                            break;
                    }
                }
                else
                {
                    switch (RootGrid.RequestedTheme)
                    {
                        case ElementTheme.Light:
                            Light.IsChecked = true;
                            break;
                        case ElementTheme.Dark:
                            Dark.IsChecked = true;
                            break;
                    }
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                switch (rb.Name)
                {
                    case "Light":
                        RootGrid.RequestedTheme = ElementTheme.Light;
                        SettingsHelper.SetSetting(AppSettings.AppTheme, rb.Name);
                        ((App)Application.Current).IsSystemTheme = false;
                        break;
                    case "Dark":
                        RootGrid.RequestedTheme = ElementTheme.Dark;
                        SettingsHelper.SetSetting(AppSettings.AppTheme, rb.Name);
                        ((App)Application.Current).IsSystemTheme = false;

                        break;
                    case "System":
                        RootGrid.RequestedTheme = ((App)App.Current).SystemTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                        SettingsHelper.ClearSetting(AppSettings.AppTheme);
                        ((App)Application.Current).IsSystemTheme = true;
                        break;
                    default:
                        RootGrid.RequestedTheme = ((App)App.Current).SystemTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                        SettingsHelper.ClearSetting(AppSettings.AppTheme);
                        ((App)Application.Current).IsSystemTheme = true;
                        break;
                }
            }

            ThemeChanged?.Invoke(this, RootGrid.RequestedTheme);
        }

        public Visibility InvertVisibility(bool input)
        {
            return input ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
