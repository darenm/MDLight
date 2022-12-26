using System;
using System.ComponentModel;

namespace MDLight.Services
{
    internal interface INavigationService : INotifyPropertyChanged
    {
        bool CanGoBack { get; set; }

        event EventHandler<EventArgs> OnBackButtonClicked;

        void RaiseOnBackButtonClicked(object sender, EventArgs args);
    }
}
