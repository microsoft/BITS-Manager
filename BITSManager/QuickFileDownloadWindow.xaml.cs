// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
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

// Set up the BITS namespaces
using BITS = BITSReference1_5;
//using BITS4 = BITSReference4_0;
//using BITS5 = BITSReference5_0;
//using BITS10_2 = BITSReference10_2;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for QuickFileDownloadControl.xaml
    /// </summary>
    public partial class QuickFileDownloadWindow : Window
    {
        bool FileHasChanged = false;
        public QuickFileDownloadWindow()
        {
            InitializeComponent();
            this.Loaded += QuickFileDownloadControl_Loaded;
        }

        public void SetJobProperties (BITS.IBackgroundCopyJob job)
        {
            uiJobProperty.SetJobProperties(job);
        }

        private void QuickFileDownloadControl_Loaded(object sender, RoutedEventArgs e)
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


        public string RemoteUri { get { return uiUri.Text; } }
        public string LocalFile { get { return uiFile.Text; } }


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
                if (file != "/")
                {
                    var dir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var fullpath = System.IO.Path.Combine(dir, file);
                    uiFile.Text = fullpath;
                }
            }
        }


        private void OnFileChangedViaKeyboard(object sender, KeyEventArgs e)
        {
            FileHasChanged = true;
        }
    }
}
