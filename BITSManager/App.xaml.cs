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
            var message = String.Format(
                BITSManager.Properties.Resources.UnhandledExceptionMessage,
                e.Exception.Message,
                e.Exception.GetType().ToString());
            MessageBox.Show(message, BITSManager.Properties.Resources.UnhandledExceptionTitle);
        }
    }
}