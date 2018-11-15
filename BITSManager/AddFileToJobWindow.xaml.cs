using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for AddFiletoJobWindow.xaml
    /// </summary>
    public partial class AddFileToJobWindow : Window
    {
        public string RemoteUri { get { return uiUri.Text; } }
        public string LocalFile { get { return uiFile.Text; } }
        bool FileHasChanged = false;

        public AddFileToJobWindow()
        {
            InitializeComponent();
            this.Loaded += AddFileToJobWindowControl_Loaded;
        }


        private void AddFileToJobWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            uiUri.Focus();
        }


        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        private void OnUriChanged(object sender, TextChangedEventArgs e)
        {
            string newUriText = uiUri.Text;
            Uri uri;
            var parseStatus = Uri.TryCreate(newUriText, UriKind.Absolute, out uri);
            if (parseStatus && uri.Segments.Length >= 1 && !FileHasChanged)
            {
                // Make a corresponding file name. If the user has changed the file text,
                // don't update it when the URL changes.
                var file = uri.Segments[uri.Segments.Length - 1];
                var dir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fullpath = System.IO.Path.Combine(dir, file);
                uiFile.Text = fullpath;
            }
        }


        private void OnFileChangedViaKeyboard(object sender, KeyEventArgs e)
        {
            FileHasChanged = true;
        }
    }
}
