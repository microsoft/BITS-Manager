// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows;
using System.Windows.Controls;

// Set up the BITS namespaces
using BITS = BITSReference1_5;

//using BITS4 = BITSReference4_0;
using BITS5 = BITSReference5_0;

//using BITS10_2 = BITSReference10_2;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for FileDetailViewControl.xaml
    /// </summary>
    public partial class FileDetailViewControl : UserControl
    {
        private BITS.IBackgroundCopyJob Job;
        private BITS.IBackgroundCopyFile File;

        public FileDetailViewControl(BITS.IBackgroundCopyJob job, BITS.IBackgroundCopyFile file)
        {
            Job = job;
            File = file;

            InitializeComponent();

            // Set the different parts of the UI
            string localName;
            File.GetLocalName(out localName);
            uiFileLocal.Text = localName;

            string remoteName;
            File.GetRemoteName(out remoteName);
            uiFileRemote.Text = remoteName;

            // Add in the current progress
            BITS._BG_FILE_PROGRESS progress;
            File.GetProgress(out progress);
            var bytes = progress.BytesTotal == ulong.MaxValue
                ? String.Format(
                    Properties.Resources.FileProgressByteCountUnknown,
                    progress.BytesTransferred)
                : String.Format(
                    Properties.Resources.FileProgressByteCount,
                    progress.BytesTransferred,
                    progress.BytesTotal);

            uiFileByteProgress.Text = bytes;

            // Get the data from the file as a IBackgroundCopyFile5 (available starting in Windows 8)
            var file5 = file as BITS5.IBackgroundCopyFile5;
            if (file5 == null)
            {
                uiFileHttpResponseData.Text = Properties.Resources.FileHttpResponseDataNotAvailable;
            }
            else
            {
                // The bits5_0.idl IDL file was modified to convert the String parameter.
                // The type was changed to WCHAR* and the resulting value is marshalled as an IntPtr.
                // The union was also given a name to match the typedef name.
                // typedef[switch_type(BITS_FILE_PROPERTY_ID)] union BITS_FILE_PROPERTY_VALUE
                // {
                //     [case(BITS_FILE_PROPERTY_ID_HTTP_RESPONSE_HEADERS )]
                //         WCHAR* String;
                //     }
                //     BITS_FILE_PROPERTY_VALUE;
                BITS5.BITS_FILE_PROPERTY_VALUE value;
                file5.GetProperty(BITS5.BITS_FILE_PROPERTY_ID.BITS_FILE_PROPERTY_ID_HTTP_RESPONSE_HEADERS, out value);
                var str = System.Runtime.InteropServices.Marshal.PtrToStringAuto(value.String);
                str = TabifyHttpHeaders.AddTabs(str);
                uiFileHttpResponseData.Text = str;
            }
        }

        private void OnOpenFile(object sender, RoutedEventArgs e)
        {
            string Filename;
            File.GetLocalName(out Filename);
            try
            {
                System.Diagnostics.Process.Start(Filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorMessage, ex.Message),
                    Properties.Resources.ErrorTitle);
            }
        }
    }
}