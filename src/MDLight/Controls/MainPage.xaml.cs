
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using MDLight.Models;
using MDLight.Services;
using MDLight.Utilities;
using MDLight.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Windows.Storage;

namespace MDLight.Controls
{
    public sealed partial class MainPage : UserControl
    {
        private INavigationService _navigationService;
        private List<Document> _documents = new List<Document>();
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


        public RelayCommand OpenCommand
        {
            get { return (RelayCommand)GetValue(OpenCommandProperty); }
            set { SetValue(OpenCommandProperty, value); }
        }

        public static readonly DependencyProperty OpenCommandProperty =
            DependencyProperty.Register("OpenCommand", typeof(int), typeof(MainPage), new PropertyMetadata(null));



        public RelayCommand NewCommand
        {
            get { return (RelayCommand)GetValue(NewCommandProperty); }
            set { SetValue(NewCommandProperty, value); }
        }

        public static readonly DependencyProperty NewCommandProperty =
            DependencyProperty.Register("NewCommand", typeof(RelayCommand), typeof(MainPage), new PropertyMetadata(null));

        public RelayCommand SaveAsCommand
        {
            get { return (RelayCommand)GetValue(SaveAsCommandProperty); }
            set { SetValue(SaveAsCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveAsCommandProperty =
            DependencyProperty.Register("SaveAsCommand", typeof(RelayCommand), typeof(MainPage), new PropertyMetadata(null));

        public RelayCommand SaveCommand
        {
            get { return (RelayCommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(RelayCommand), typeof(MainPage), new PropertyMetadata(null));

        public RelayCommand EditCommand
        {
            get { return (RelayCommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(RelayCommand), typeof(MainPage), new PropertyMetadata(null));


        public MainPage()
        {
            this.InitializeComponent();

            DataContext = this;
            _navigationService = ((App)Application.Current).Services.GetService<INavigationService>();
            _navigationService.OnBackButtonClicked += _navigationService_OnBackButtonClicked;

            OpenCommand = new RelayCommand(OpenCommand_Execute);
            NewCommand = new RelayCommand(NewCommand_Execute);
            EditCommand = new RelayCommand(EditCommand_Execute, EditCommand_CanExecute);
            SaveCommand = new RelayCommand(SaveCommand_Execute, SaveCommand_CanExecute);
            SaveAsCommand = new RelayCommand(SaveAsCommand_Execute, SaveAsCommand_CanExecute);

        }

        private bool SaveAsCommand_CanExecute()
        {
            return _documents.Count > 0;
        }

        private bool SaveCommand_CanExecute()
        {
            return _documents.Count > 0;
        }

        private bool EditCommand_CanExecute()
        {
            return _documents.Count > 0;
        }

        private void SaveAsCommand_Execute()
        {
        }

        private async void SaveCommand_Execute()
        {
            if (NotesTabs.SelectedItem != null)
            {
                var markdownView = ((TabViewItem)NotesTabs.SelectedItem).Content as MarkdownView;
                if (markdownView != null)
                {
                    markdownView.SetEdit(false);
                    var document = markdownView.Document;
                    await FileIO.WriteTextAsync(document.File, document.Contents);
                }
            }
        }

        private void EditCommand_Execute()
        {
            if (NotesTabs.SelectedItem != null)
            {
                var markdownView = ((TabViewItem)NotesTabs.SelectedItem).Content as MarkdownView;
                if (markdownView != null)
                {
                    markdownView.SetEdit(true);
                }
            }
        }

        private async void OpenCommand_Execute()
        {
            var file = await ((App)Application.Current).MainWindow.PickFile(new string[] { ".md" });
            if (file != null)
            {
                await OpenTab(file);
            }

            CheckCommands();
        }

        private void CheckCommands()
        {
            EditCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            SaveAsCommand.NotifyCanExecuteChanged();
        }

        private async Task OpenTab(StorageFile file)
        {
            var document = new Document()
            {
                FileName = file.Name,
                File = file,
                Contents = await FileIO.ReadTextAsync(file)
            };

            _documents.Add(document);

            var newTab = CreateTab(document);
            NotesTabs.TabItems.Add(newTab);
            NotesTabs.SelectedItem = newTab;
        }

        private object CreateTab(Document document)
        {
            TabViewItem newItem = new TabViewItem
            {
                Header = document.Title,
                IconSource = new SymbolIconSource() { Symbol = Symbol.Document },
                Tag = document,
                Content = new MarkdownView() { Tag = document },
            };

            newItem.IconSource.Foreground = _orangeBrush;

            return newItem;
        }

        private void NewCommand_Execute()
        {
            var file = ((App)Application.Current).MainWindow.PickFile(new string[] { ".md" });
        }

        private void _navigationService_OnBackButtonClicked(object sender, System.EventArgs e)
        {
            ShowSettings = false;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            OpenCommand.Execute(null);
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            _documents.Remove(args.Tab.Tag as Document);
            NotesTabs.TabItems.Remove(args.Item);
            CheckCommands();
        }

        private void OnElementClicked(object sender, RoutedEventArgs e)
        {

        }

        private void onSettingsClicked(object sender, RoutedEventArgs e)
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
    }
}
