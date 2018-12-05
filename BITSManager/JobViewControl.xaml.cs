// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System.Windows.Controls;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for JobViewControl.xaml
    /// </summary>
    public partial class JobViewControl : UserControl
    {
        /// <summary>
        /// A JobViewControl is a view into a single BITS job. The BITS job is available to the main
        /// program (for example, to list the files in the job)
        /// </summary>
        public BITS.IBackgroundCopyJob Job { get; internal set; } = null;

        public bool Updated { get; set; } = false; // Used by MainWindow to decide if this job is "fresh"
        public bool JobIsFinal { get; private set; } = false;

        public void MarkAsOld()
        {
            uiJobOld.Text = "⧆";
        }

        public JobViewControl(BITS.IBackgroundCopyJob job)
        {
            Job = job;
            InitializeComponent();

            // Set the different parts of the UI
            string displayName;
            Job.GetDisplayName(out displayName);
            uiJobName.Text = displayName;

            string description;
            Job.GetDescription(out description);
            uiJobDescription.Text = description;

            UpdateState();
        }

        public void UpdateState()
        {
            BITS.BG_JOB_STATE jobState;
            Job.GetState(out jobState);
            uiJobState.Text = BitsConversions.ConvertJobStateToIconString(jobState);
            uiJobState.ToolTip = BitsConversions.ConvertJobStateToString(jobState);
            JobIsFinal = (jobState == BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED)
                || (jobState == BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED);
        }
    }
}