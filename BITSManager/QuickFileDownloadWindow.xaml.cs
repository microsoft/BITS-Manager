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
    public partial class QuickFileDownloadWindow : Window, BITS.IBackgroundCopyCallback
    {
        public BITS.IBackgroundCopyJob Job { get; internal set; } = null;

        bool FileHasChanged = false;
        public QuickFileDownloadWindow()
        {
            InitializeComponent();
            this.Loaded += QuickFileDownloadControl_Loaded;
        }

        /// <summary>
        /// Sets job properties using settings from the SetJobProperties control
        /// </summary>
        /// <param name="job"></param>
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
            Job = null; // Clear out the old value, if any.
            this.DialogResult = true;
            if (RemoteUri == "")
            {
                MessageBox.Show(Properties.Resources.ErrorEmptyRemoteUrl, Properties.Resources.ErrorTitle);
                return;
            }
            if (LocalFile == "")
            {
                MessageBox.Show(Properties.Resources.ErrorEmptyLocalFile, Properties.Resources.ErrorTitle);
                return;
            }
            Job = DownloadFile(RemoteUri, LocalFile);
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

        /// <summary>
        /// Demonstrate how to poll for a job to be finished. A job has to be in a
        /// final state to be finished.
        /// </summary>
        /// <param name="job">Input job to wait for completion on.</param>
        private void Poll(BITS.IBackgroundCopyJob job)
        {
            // Poll for the job to be complete in a separate thread.
            new System.Threading.Thread(() =>
            {
                try
                {
                    bool jobIsFinal = false;
                    while (!jobIsFinal)
                    {
                        BITS.BG_JOB_STATE state;
                        job.GetState(out state);
                        switch (state)
                        {
                            case BITS.BG_JOB_STATE.BG_JOB_STATE_ERROR:
                            case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED:
                                job.Complete();
                                break;

                            case BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED:
                            case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED:
                                jobIsFinal = true;
                                break;
                            default:
                                Task.Delay(500); // delay a little bit
                                break;
                        }
                    }
                    // Job is in a final state (cancelled or acknowledged)
                }
                catch (Exception)
                {
                    ; // Handle job exception
                }
            }
            ).Start();

        }

        /// <summary>
        /// Demonstrate minimal code for downloading a file using BITS.
        /// </summary>
        /// <param name="URL">Remote URL to download from; e.g. "https://aka.ms/WinServ16/StndPDF"</param>
        /// <param name="filename">Local file name to download to; e.g. @"C:\Server2016.pdf" </param>
        private BITS.IBackgroundCopyJob DownloadFile(string URL, string filename)
        {
            var mgr = new BITS.BackgroundCopyManager1_5();
            BITS.GUID jobGuid;
            BITS.IBackgroundCopyJob job;
            mgr.CreateJob("Quick download", BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, 
                out jobGuid, out job);
            job.AddFile(URL, filename);
            SetJobProperties(job); // Set job properties as needed
            job.SetNotifyInterface(this); // Call JobTransferred, JobError and JobModification
            job.Resume();
            // Job is now running. We can exit and it will continue automatically.
            return job; // Return the job that was created
        }

        public void JobTransferred(BITS.IBackgroundCopyJob pJob)
        {
            pJob.Complete();
        }

        public void JobError(BITS.IBackgroundCopyJob pJob, BITS.IBackgroundCopyError pError)
        {
            pJob.Complete();
        }

        public void JobModification(BITS.IBackgroundCopyJob pJob, uint dwReserved)
        {
            // Don't need to do anything on job modification
        }
    }
}
