// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MDLight.Utilities;

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

namespace MDLight
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            WindowHelper.RegisterWindowMinMax(this);
            WindowHelper.MinWindowWidth = 400;
            WindowHelper.MinWindowHeight = 400;
            WindowHelper.SetWindowSize(this, 900, 600);
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //myButton.Content = "Clicked";
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {

        }

        private void OnElementClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
