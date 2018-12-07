// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System.Windows;
using System.Windows.Controls;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for CreateNewJobDialog.xaml
    /// </summary>
    public partial class CreateNewJobWindow : Window
    {
        /// <summary>
        /// JobName that the user typed in; will be used by the caller to create the BITS job.
        /// </summary>
        public string JobName { get { return uiJobName.Text; } }

        public CreateNewJobWindow()
        {
            InitializeComponent();
            Loaded += CreateNewJobWindow_Loaded;
        }

        public void SetJobProperties(BITS.IBackgroundCopyJob job)
        {
            uiJobProperty.SetJobProperties(job);
        }

        private void CreateNewJobWindow_Loaded(object sender, RoutedEventArgs e)
        {
            uiJobName.Focus();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public BITS.BG_JOB_TYPE JobType
        {
            get
            {
                // The value of the tag is a fixed string; it's not presented to the user and is not localized.
                switch ((uiJobType.SelectedValue as ComboBoxItem).Tag as string)
                {
                    default:
                    case "Download": return BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD;
                    case "Upload": return BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD;
                    case "UploadReply": return BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD_REPLY;
                }
            }
        }
    }
}
