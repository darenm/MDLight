using CommunityToolkit.Mvvm.ComponentModel;

namespace MDLight.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private bool _showSettings;

        public bool ShowSettings { get => _showSettings; set => SetProperty(ref _showSettings, value); }

        public MainViewModel()
        {
            ShowSettings = false;
        }
    }
}
