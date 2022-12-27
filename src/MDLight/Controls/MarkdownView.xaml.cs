// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MDLight.Models;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDLight.Controls
{
    public sealed partial class MarkdownView : UserControl
    {
        private Document _document;

        internal Document Document { get => _document; set => _document = value; }

        public MarkdownView()
        {
            this.InitializeComponent();
            this.Loaded += MarkdownView_Loaded;
        }

        private void MarkdownView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tag is Document document)
            {
                Document = document;
                DataContext= Document;
                SetEdit(false);
            }
        }

        public void SetEdit(bool isEditing)
        {
            if (isEditing)
            {
                MarkdownText.Visibility= Visibility.Collapsed;
                EditingText.Visibility= Visibility.Visible;
            }
            else
            {
                MarkdownText.Visibility= Visibility.Visible;
                EditingText.Visibility= Visibility.Collapsed;
            }
        }

    }
}
