using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using MDLight.Controls;
using MDLight.Messages;
using MDLight.Models;
using MDLight.Services;
using MDLight.Utilities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using Windows.Storage;
using Windows.System;

namespace MDLight.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private IWindowsService _windowsService;
        private INavigationService _navigationService;

        private bool _showSettings;
        public bool ShowSettings { get => _showSettings; set => SetProperty(ref _showSettings, value); }

        private RelayCommand _openCommand;
        public RelayCommand OpenCommand { get => _openCommand; set => SetProperty(ref _openCommand, value); }

        private RelayCommand _newCommand;
        public RelayCommand NewCommand { get => _newCommand; set => SetProperty(ref _newCommand, value); }

        private RelayCommand _editCommand;
        public RelayCommand EditCommand { get => _editCommand; set => SetProperty(ref _editCommand, value); }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand { get => _saveCommand; set => SetProperty(ref _saveCommand, value); }

        private RelayCommand _saveAsCommand;
        public RelayCommand SaveAsCommand { get => _saveAsCommand; set => SetProperty(ref _saveAsCommand, value); }

        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand { get => _settingsCommand; set => SetProperty(ref _settingsCommand, value); }

        public ObservableCollection<MarkdownDocument> Documents = new ObservableCollection<MarkdownDocument>();

        private int _selectedTabIdex = 0;
        public int SelectedTabIndex
        {
            get => _selectedTabIdex; set
            {
                SetProperty(ref _selectedTabIdex, value);
                OnPropertyChanged(nameof(CurrentDocument));
                OnPropertyChanged(nameof(CurrentDocumentIsEditing));
            }
        }

        public MarkdownDocument CurrentDocument => Documents.Any() && SelectedTabIndex >=0 ? Documents[SelectedTabIndex] : null;
        public bool CurrentDocumentIsEditing => CurrentDocument != null ? CurrentDocument.IsEdit : false;

        public MainViewModel(IWindowsService windowsService)
        {
            ShowSettings = false;

            _windowsService = ServicesResolver.Services.GetService<IWindowsService>();
            _navigationService = ServicesResolver.Services.GetService<INavigationService>();

            OpenCommand = new RelayCommand(OpenCommand_Execute);
            NewCommand = new RelayCommand(NewCommand_Execute);
            EditCommand = new RelayCommand(EditCommand_Execute, EditCommand_CanExecute);
            SaveCommand = new RelayCommand(SaveCommand_Execute, SaveCommand_CanExecute);
            SaveAsCommand = new RelayCommand(SaveAsCommand_Execute, SaveAsCommand_CanExecute);
            SettingsCommand = new RelayCommand(SettingsCommand_Execute, SettingsCommand_CanExecute);
            _windowsService = windowsService;
        }

        private bool SettingsCommand_CanExecute()
        {
            return !ShowSettings;
        }

        private void SettingsCommand_Execute()
        {
            ShowSettings = true;
            WeakReferenceMessenger.Default.Send(new ShowSettingsMessage(ShowSettings));
        }

        private bool SaveAsCommand_CanExecute()
        {
            return Documents.Count > 0;
        }

        private bool SaveCommand_CanExecute()
        {
            return Documents.Count > 0;
        }

        private bool EditCommand_CanExecute()
        {
            return Documents.Count > 0;
        }

        private void SaveAsCommand_Execute()
        {
        }

        private async void SaveCommand_Execute()
        {
            Documents[SelectedTabIndex].IsEdit = false;
            if (Documents[SelectedTabIndex].File != null)
            {
                await FileIO.WriteTextAsync(Documents[SelectedTabIndex].File, Documents[SelectedTabIndex].Contents);
            }
            else
            {
                var saveFile = await _windowsService.MainWindow.PickFileSaveAs(
                    new System.Collections.Generic.Dictionary<string, string[]> { { "Markdown", new[] { ".md" } } },
                    Documents[SelectedTabIndex].FileBytes,
                    "Untitled.md");
                if (saveFile != null)
                {
                    Documents[SelectedTabIndex].File = saveFile;
                }
            }
        }

        private void EditCommand_Execute()
        {
            Documents[SelectedTabIndex].IsEdit = !Documents[SelectedTabIndex].IsEdit;

            if (!Documents[SelectedTabIndex].IsEdit)
            {
                SaveCommand_Execute();
            }
        }

        public void CloseTabRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            Documents.Remove(args.Item as MarkdownDocument);
            CheckCommands();
        }

        private bool CloseCommand_CanExecute()
        {
            return Documents.Count > 0;
        }

        private async void OpenCommand_Execute()
        {
            var file = await _windowsService.MainWindow.PickFile(new string[] { ".md" });
            if (file != null)
            {
                await OpenTab(file);
            }

            CheckCommands();
        }

        private async Task OpenTab(StorageFile file)
        {
            var document = new MarkdownDocument()
            {
                FileName = file.Name,
                File = file,
                Contents = await FileIO.ReadTextAsync(file)
            };

            Documents.Add(document);
            SelectedTabIndex = Documents.Count - 1;
        }

        private void NewCommand_Execute()
        {
            var document = new MarkdownDocument()
            {
                FileName = "Untitled",
                File = null,
                Contents = ""
            };

            Documents.Add(document);
            SelectedTabIndex = Documents.Count - 1;
            Documents[SelectedTabIndex].IsEdit = true;
            CheckCommands();
        }

        private void CheckCommands()
        {
            EditCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            SaveAsCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CurrentDocumentIsEditing));
        }
    }
}
