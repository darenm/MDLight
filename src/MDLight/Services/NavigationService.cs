using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace MDLight.Services
{
    internal class NavigationService : ObservableObject, INavigationService
    {
        private bool _canGoBack;

        public bool CanGoBack { get => _canGoBack; set => SetProperty(ref _canGoBack, value); }

        public event EventHandler<EventArgs> OnBackButtonClicked;

        public void RaiseOnBackButtonClicked(object sender, EventArgs args)
        {
            OnBackButtonClicked?.Invoke(sender, args);
        }
    }
}
