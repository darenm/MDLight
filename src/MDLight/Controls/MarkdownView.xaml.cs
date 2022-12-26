// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MDLight.Models;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDLight.Controls
{
    public sealed partial class MarkdownView : UserControl
    {
        public MarkdownView()
        {
            this.InitializeComponent();
            this.Loaded += MarkdownView_Loaded;
        }

        private void MarkdownView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tag is Document document)
            {
                Content.Text = document.Contents;
            }
        }
    }
}
