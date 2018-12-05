// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IRefreshJobList, BITS.IBackgroundCopyCallback
    {
        private const uint BG_JOB_ENUM_ALL_USERS = 1;
        private uint _jobEnumType = 0; // is either 0 or BG_JOB_ENUM_ALL_USERS
        private BITS.IBackgroundCopyManager _mgr = null;
        private DispatcherTimer _timer;
        private bool _shouldNotifyUserOnAccessError = false;

        public static readonly RoutedCommand QuickFileDownloadCommand = new RoutedUICommand(
            "QuickFileDownload", 
            "QuickFileDownloadCommand", 
            typeof(MainWindow), 
            new InputGestureCollection(
                new InputGesture[] { new KeyGesture(Key.K, ModifierKeys.Control)}
                )
            );

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _mgr = new BITS.BackgroundCopyManager1_5();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format(
                    Properties.Resources.ErrorCantConnectToBitsException,
                    ex.HResult,
                    ex.Message));
            }
            if (_mgr == null)
            {
                MessageBox.Show(Properties.Resources.ErrorCantConnectToBits, Properties.Resources.ErrorTitle);
            }

            RefreshJobList();
            bool focusStatus = uiJobList.Focus();
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            _timer.Start();
            uiJobDetails.RefreshJobList = this;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshJobList();
        }

        public void RefreshJobList()
        {
            var currIndex = uiJobList.SelectedIndex;

            // Get the iterator. Handle the throw when the user doesn't have permissions.
            BITS.IEnumBackgroundCopyJobs jobsEnum = null;
            try
            {
                _mgr.EnumJobs(_jobEnumType, out jobsEnum);
            }
            catch (Exception ex)
            {
                // The most common error is trying to enumerate all users jobs
                // when you are not running as admin. Don't keep telling the user
                // that they are not the admin.
                if (_jobEnumType == BG_JOB_ENUM_ALL_USERS && (uint)ex.HResult == 0x80070005) // E_ACCESS_DENIED
                {
                    if (_shouldNotifyUserOnAccessError)
                    {
                        _shouldNotifyUserOnAccessError = false; // only display this dialog once.
                        MessageBox.Show(
                            Properties.Resources.ErrorInsufficientPrivilegesMessage,
                            Properties.Resources.ErrorInsufficientPrivilegesTitle);
                    }
                }
                else
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorMessage, ex.Message),
                        Properties.Resources.ErrorTitle);
                }
            }
            if (jobsEnum == null)
            {
                return;
            }

            // Set all jobs as not updated at the start. I'll update each active
            // job, and later mark the inactive jobs as old.
            foreach (var item in uiJobList.Items)
            {
                var control = item as JobViewControl;
                control.Updated = false;
            }

            uint njobFetched = 0;
            BITS.IBackgroundCopyJob job = null;

            do
            {
                jobsEnum.Next(1, out job, ref njobFetched); // Can only pull a single job out at a time
                if (njobFetched > 0)
                {
                    // Do something with the job
                    var control = new JobViewControl(job);
                    control.Updated = true;
                    int idx = GetJobIndex(job);
                    if (idx >= 0)
                    {
                        uiJobList.Items[idx] = control;
                    }
                    else
                    {
                        uiJobList.Items.Add(control);
                    }
                }
            }
            while (njobFetched > 0);

            foreach (var item in uiJobList.Items)
            {
                var control = item as JobViewControl;
                if (!control.Updated)
                {
                    control.MarkAsOld();
                    if (!control.JobIsFinal) // Once it's final, it will never change
                    {
                        control.UpdateState();
                    }
                }
            }

            if (uiJobList.Items.Count > 0)
            {
                uiJobDetails.Visibility = Visibility.Visible;
                var oldIndex = uiJobList.SelectedIndex;
                uiJobList.SelectedIndex = currIndex >= 0 ? currIndex : 0;
                if (uiJobList.SelectedIndex == oldIndex)
                {
                    // Refresh the currently selected item. It won't auto-refresh
                    // because we didn't change the selection.
                    var control = uiJobList.SelectedItem as JobViewControl;
                    uiJobDetails.SetJob(control.Job);
                }
            }
            else
            {
                uiJobDetails.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Given a job, return the index in the visible job list that matches the job.
        /// Uses the job guid as the key.
        /// </summary>
        /// <param name="job">The IBackgroundCopyJob to look for</param>
        /// <returns>An index 0..n for a job that's found and -1 if not found</returns>
        private int GetJobIndex(BITS.IBackgroundCopyJob job)
        {
            BITS.GUID searchFor;
            job.GetId(out searchFor);
            for (int i = 0; i < uiJobList.Items.Count; i++)
            {
                var control = uiJobList.Items[i] as JobViewControl;
                BITS.GUID id;
                control.Job.GetId(out id);
                if (GuidEqual(searchFor, id))
                {
                    return i;
                }
            }
            return -1;
        }

        private static bool GuidEqual(BITS.GUID a, BITS.GUID b)
        {
            bool field4equal = true;
            if (a.Data4.Length != b.Data4.Length)
            {
                field4equal = false;
            }
            else
            {
                for (int i = 0; i < a.Data4.Length; i++)
                {
                    if (a.Data4[i] != b.Data4[i])
                    {
                        field4equal = false;
                    }
                }
            }

            var Retval = (a.Data1 == b.Data1
                && a.Data2 == b.Data2
                && a.Data3 == b.Data3
                && field4equal);
            return Retval;
        }

        private void OnJobSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }
            var control = e.AddedItems[0] as JobViewControl;
            uiJobList.ScrollIntoView(control);
            uiJobDetails.SetJob(control.Job);
        }

        /// <summary>
        /// QuickFileDownload is a dialog box that lets you paste in a URL + Path to download a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuQuickFileDownload(object sender, RoutedEventArgs e)
        {
            var dlg = new QuickFileDownloadWindow();
            dlg.Owner = this;
            var result = dlg.ShowDialog();
            var job = dlg.Job;

            // Update the UI with the new job (if any)
            if (job != null)
            {
                RefreshJobList();
                var idx = GetJobIndex(job);
                uiJobList.SelectedIndex = idx;
            }
        }

        private void OnMenuCreateNewJob(object sender, RoutedEventArgs e)
        {
            var dlg = new CreateNewJobWindow();
            dlg.Owner = this;
            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value && _mgr != null)
            {
                var jobName = dlg.JobName;
                var jobType = dlg.JobType;
                BITS.GUID jobId;
                BITS.IBackgroundCopyJob job;
                _mgr.CreateJob(jobName, jobType, out jobId, out job);
                dlg.SetJobProperties(job);
                RefreshJobList();

                // Select the newly-created job
                var idx = GetJobIndex(job);
                uiJobList.SelectedIndex = idx;
            }
        }

        private void OnMenuRefresh(object sender, RoutedEventArgs e)
        {
            RefreshJobList();
        }

        private void OnMenuAllUsers(object sender, RoutedEventArgs e)
        {
            _shouldNotifyUserOnAccessError = true;
            _jobEnumType = uiMenuAllUsers.IsChecked ? BG_JOB_ENUM_ALL_USERS : 0;
            RefreshJobList();
        }

        // Callbacks for when a job is transferred
        public void JobTransferred(BITS.IBackgroundCopyJob pJob)
        {
            Dispatcher.Invoke(() => { RefreshJobList(); });
        }

        public void JobError(BITS.IBackgroundCopyJob pJob, BITS.IBackgroundCopyError pError)
        {
            Dispatcher.Invoke(() => { RefreshJobList(); });
        }

        public void JobModification(BITS.IBackgroundCopyJob pJob, uint dwReserved)
        {
            Dispatcher.Invoke(() => { RefreshJobList(); });
        }

        private void OnMenuFileExit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnMenuHelpAbout(object sender, RoutedEventArgs e)
        {
            var moreUrl = "https://github.com/Microsoft/BITS-Manager";
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var text = String.Format(Properties.Resources.AboutMessage, version, moreUrl);
            MessageBox.Show(text, Properties.Resources.AboutTitle);
        }
    }
}