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
        public MarkdownDocument Document
        {
            get { return (MarkdownDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Document.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(MarkdownDocument), typeof(MarkdownView), new PropertyMetadata(null));


        public MarkdownView()
        {
            this.InitializeComponent();
        }

        public Visibility InvertVisibility(bool input)
        {
            return input ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
