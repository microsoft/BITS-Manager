// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // This doesn't set e.Handled because we want to show a reasonable dialog box on exception,
            // but we are still willing to have the app crash
            var message = String.Format(
                BITSManager.Properties.Resources.UnhandledExceptionMessage,
                e.Exception.Message,
                e.Exception.GetType().ToString());
            MessageBox.Show(message, BITSManager.Properties.Resources.UnhandledExceptionTitle);
        }
    }
}
