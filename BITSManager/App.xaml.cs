using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            // This is how to force the app to use a specific UI culture.
            // The culture must be a valid  BCP-47  https://tools.ietf.org/html/bcp47 .
            // These in turn must be ISO 639 codes. 
            // Only cultures supported by the app should be used!
            // You should only set to a culture supported by the OS; otherwise the BITS
            // error methods will fail to get error messages for BITS errors.

            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es");

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var message = String.Format (BITSManager.Properties.Resources.UnhandledExceptionMessage, e.Exception.Message, e.Exception.GetType().ToString());
            MessageBox.Show(message, BITSManager.Properties.Resources.UnhandledExceptionTitle);
        }
    }
}
