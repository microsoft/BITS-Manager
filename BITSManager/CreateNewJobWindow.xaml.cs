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
using System.Windows.Shapes;

// Set up the BITS namespaces
using BITS = BITSReference1_5;
//using BITS4 = BITSReference4_0;
//using BITS5 = BITSReference5_0;
//using BITS10_2 = BITSReference10_2;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for CreateNewJobDialog.xaml
    /// </summary>
    public partial class CreateNewJobWindow : Window
    {
        public CreateNewJobWindow()
        {
            InitializeComponent();
            this.Loaded += CreateNewJobWindow_Loaded;
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
            this.DialogResult = false;
        }


        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string JobName { get { return uiJobName.Text; } }
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
