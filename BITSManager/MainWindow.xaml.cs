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
    public partial class MainWindow : Window, BITS.IBackgroundCopyCallback
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
                new InputGesture[] { new KeyGesture(Key.K, ModifierKeys.Control) }
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
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show(
                    String.Format(
                    Properties.Resources.ErrorCantConnectToBitsException,
                    ex.HResult,
                    ex.Message));
            }
            if (_mgr == null)
            {
                _uiJobDetails.Visibility = Visibility.Hidden;
                UpdateMenu();
                MessageBox.Show(Properties.Resources.ErrorCantConnectToBits, Properties.Resources.ErrorTitle);
            }
            else
            {
                RefreshJobList();
                _uiJobList.Focus();
                _timer = new DispatcherTimer();
                _timer.Tick += Timer_Tick;
                _timer.Interval = new TimeSpan(0, 0, 10);
                _timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshJobList();
        }

        public void RefreshJobList()
        {
            if (_mgr == null)
            {
                return;
            }

            var currIndex = _uiJobList.SelectedIndex;

            // Get the iterator. Handle the throw when the user doesn't have permissions.
            BITS.IEnumBackgroundCopyJobs jobsEnum = null;
            try
            {
                _mgr.EnumJobs(_jobEnumType, out jobsEnum);
            }
            catch (System.UnauthorizedAccessException ex)
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

            // Set all jobs as not updated at the start. The code will then update
            // each active job, and later mark the inactive jobs as old.
            foreach (var item in _uiJobList.Items)
            {
                var control = item as JobViewControl;
                control.Updated = false;
            }

            uint jobFetchedCount = 0;
            do
            {
                BITS.IBackgroundCopyJob job = null;
                jobsEnum.Next(1, out job, ref jobFetchedCount); // Can only pull a single job out at a time
                if (jobFetchedCount > 0)
                {
                    var control = new JobViewControl(job);
                    control.Updated = true;
                    int idx = GetJobIndex(job);
                    if (idx >= 0)
                    {
                        _uiJobList.Items[idx] = control;
                    }
                    else
                    {
                        _uiJobList.Items.Add(control);
                    }
                }
            }
            while (jobFetchedCount > 0);

            foreach (var item in _uiJobList.Items)
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

            if (_uiJobList.Items.Count > 0)
            {
                _uiJobDetails.Visibility = Visibility.Visible;
                var oldIndex = _uiJobList.SelectedIndex;
                _uiJobList.SelectedIndex = currIndex >= 0 ? currIndex : 0;
                if (_uiJobList.SelectedIndex == oldIndex)
                {
                    // Refresh the currently selected item. It won't auto-refresh
                    // because we didn't change the selection.
                    var control = _uiJobList.SelectedItem as JobViewControl;
                    _uiJobDetails.SetJob(control.Job);
                }
            }
            else
            {
                _uiJobDetails.Visibility = Visibility.Hidden;
            }

            UpdateMenu();
        }

        private void UpdateMenu()
        {
            var jobMenuEnabled = true;
            if (_uiJobList.Items.Count > 0 && _uiJobList.SelectedItem != null)
            {
                // Update the jobs menus based on the job state
                var job = (_uiJobList.SelectedItem as JobViewControl).Job;
                BITS.BG_JOB_STATE state;
                job.GetState(out state);
                switch (state)
                {
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED:
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED:
                    jobMenuEnabled = false;
                    break;
                }
            }
            else
            {
                jobMenuEnabled = false;
            }

            _uiMenuJobCancel.IsEnabled = jobMenuEnabled;
            _uiMenuJobComplete.IsEnabled = jobMenuEnabled;
            _uiMenuJobResume.IsEnabled = jobMenuEnabled;
            _uiMenuJobSuspend.IsEnabled = jobMenuEnabled;
            _uiMenuJobAddFile.IsEnabled = jobMenuEnabled;
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
            for (int i = 0; i < _uiJobList.Items.Count; i++)
            {
                var control = _uiJobList.Items[i] as JobViewControl;
                BITS.GUID id;
                control.Job.GetId(out id);
                if (searchFor.GuidEquals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnJobSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }
            var control = e.AddedItems[0] as JobViewControl;
            _uiJobList.ScrollIntoView(control);
            _uiJobDetails.SetJob(control.Job);
            UpdateMenu();
        }

        /// <summary>
        /// QuickFileDownload is a dialog box that lets you paste in a URL + Path to download a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuJobQuickFileDownload(object sender, RoutedEventArgs e)
        {
            var dlg = new QuickFileDownloadWindow(_mgr);
            dlg.Owner = this;
            var result = dlg.ShowDialog();
            var job = dlg.Job;

            // Update the UI with the new job (if any)
            if (job != null)
            {
                RefreshJobList();
                var idx = GetJobIndex(job);
                _uiJobList.SelectedIndex = idx;
            }
        }

        private void OnMenuJobCreateNewJob(object sender, RoutedEventArgs e)
        {
            if (_mgr == null)
            {
                return;
            }

            var dlg = new CreateNewJobWindow();
            dlg.Owner = this;
            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var jobName = dlg.JobName;
                var jobType = dlg.JobType;
                BITS.GUID jobId;
                BITS.IBackgroundCopyJob job;
                _mgr.CreateJob(jobName, jobType, out jobId, out job);
                try
                {
                dlg.SetJobProperties(job);
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorMessage, ex.Message), Properties.Resources.ErrorTitle);
                    // No need to cancel; the job will show up in the job list and
                    // will be selected. The user should deal with it as they see fit.
                }

                RefreshJobList();

                // Select the newly-created job
                var idx = GetJobIndex(job);
                _uiJobList.SelectedIndex = idx;
            }
        }

        private void OnMenuRefresh(object sender, RoutedEventArgs e)
        {
            RefreshJobList();
        }

        private void OnMenuAllUsers(object sender, RoutedEventArgs e)
        {
            _shouldNotifyUserOnAccessError = true;
            _jobEnumType = _uiMenuAllUsers.IsChecked ? BG_JOB_ENUM_ALL_USERS : 0;
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

        private void OnSuspend(object sender, RoutedEventArgs e)
        {
            var job = _uiJobDetails.Job;
            try
            {
                job?.Suspend();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show(
                    String.Format(Properties.Resources.ErrorMessage, ex.Message),
                    String.Format(Properties.Resources.ErrorWhenTitle, Properties.Resources.JobButtonSuspend)
                    );
            }
            RefreshJobList();
        }

        private void OnMenuJobCancel(object sender, RoutedEventArgs e)
        {
            var job = _uiJobDetails.Job;
            try
            {
                job?.Cancel();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show(
                    String.Format(Properties.Resources.ErrorMessage, ex.Message),
                    String.Format(Properties.Resources.ErrorWhenTitle, Properties.Resources.JobButtonCancel)
                    );
            }
            RefreshJobList();
        }

        private void OnMenuJobComplete(object sender, RoutedEventArgs e)
        {
            var job = _uiJobDetails.Job;
            try
            {
                job?.Complete();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show(
                    String.Format(Properties.Resources.ErrorMessage, ex.Message),
                    String.Format(Properties.Resources.ErrorWhenTitle, Properties.Resources.JobButtonComplete)
                    );
            }
            RefreshJobList();
        }

        private void OnMenuJobResume(object sender, RoutedEventArgs e)
        {
            var job = _uiJobDetails.Job;
            try
            {
                job?.Resume();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show(
                    String.Format(Properties.Resources.ErrorMessage, ex.Message),
                    String.Format(Properties.Resources.ErrorWhenTitle, Properties.Resources.JobButtonResume)
                    );
            }
            RefreshJobList();
        }

        private void OnMenuJobSuspend(object sender, RoutedEventArgs e)
        {
            var job = _uiJobDetails.Job;
            try
            {
                job?.Suspend();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show(
                    String.Format(Properties.Resources.ErrorMessage, ex.Message),
                    String.Format(Properties.Resources.ErrorWhenTitle, Properties.Resources.JobButtonSuspend)
                    );
            }
            RefreshJobList();
        }

        private void OnMenuJobAddFile(object sender, RoutedEventArgs e)
        {
            var job = _uiJobDetails.Job;
            if (job == null)
            {
                return;
            }
            var dlg = new AddFileToJobWindow();
            dlg.Owner = Window.GetWindow(this);
            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var remoteUri = dlg.RemoteUri;
                var localFile = dlg.LocalFile;
                if (!string.IsNullOrEmpty(remoteUri) && !string.IsNullOrEmpty(localFile))
                {
                    try
                    {
                        job.AddFile(remoteUri, localFile);
                    }
                    catch (System.Runtime.InteropServices.COMException ex)
                    {
                        var message = String.Format(
                            Properties.Resources.JobCantAddFileMessage,
                            localFile,
                            ex.Message);
                        MessageBox.Show(message, Properties.Resources.JobCantAddFileTitle);
                    }
                }
            }
            RefreshJobList();
        }
    }
}
