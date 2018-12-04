// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows.Controls;

// Set up the BITS namespaces
using BITS = BITSReference1_5;

//using BITS4 = BITSReference4_0;
using BITS5 = BITSReference5_0;

//using BITS10_2 = BITSReference10_2;

namespace BITSManager
{
    /// <summary>
    /// Interaction logic for SetJobPropertyControl.xaml
    /// </summary>
    public partial class SetJobPropertyControl : UserControl
    {
        public SetJobPropertyControl()
        {
            InitializeComponent();
        }

        public void SetJobProperties(BITS.IBackgroundCopyJob job)
        {
            var job5 = job as BITS5.IBackgroundCopyJob5;
            if (job5 != null) // job5 will be null on, e.g., Windows 7 and earlier.
            {
                // Set the job properties.
                var costs = JobCosts;
                if (costs.HasValue)
                {
                    var value = new BITS5.BITS_JOB_PROPERTY_VALUE();
                    value.Dword = (UInt32)costs.Value;
                    job5.SetProperty(BITS5.BITS_JOB_PROPERTY_ID.BITS_JOB_PROPERTY_ID_COST_FLAGS, value);
                }

                var isDynamic = JobIsDynamic;
                if (isDynamic.HasValue)
                {
                    var value = new BITS5.BITS_JOB_PROPERTY_VALUE();
                    value.Enable = isDynamic.Value ? 1 : 0;
                    job5.SetProperty(BITS5.BITS_JOB_PROPERTY_ID.BITS_JOB_PROPERTY_DYNAMIC_CONTENT, value);
                }

                var isHighPerformance = JobIsHighPerformance;
                if (isHighPerformance.HasValue)
                {
                    var value = new BITS5.BITS_JOB_PROPERTY_VALUE();
                    value.Enable = isHighPerformance.Value ? 1 : 0;
                    job5.SetProperty(BITS5.BITS_JOB_PROPERTY_ID.BITS_JOB_PROPERTY_HIGH_PERFORMANCE, value);
                }
            }

            var priority = JobPriority;
            if (priority.HasValue)
            {
                job.SetPriority(priority.Value);
            }

            var authScheme = AuthScheme;
            if (authScheme.HasValue)
            {
                var job2 = (BITS.IBackgroundCopyJob2)job; // Job2 exists on all supported version of Windows.
                var credentials = new BITS.BG_AUTH_CREDENTIALS();
                credentials.Scheme = (BITS.BG_AUTH_SCHEME)authScheme.Value;
                credentials.Target = BITS.BG_AUTH_TARGET.BG_AUTH_TARGET_SERVER;
                // This app doesn't support setting proxy auth.
                credentials.Credentials.Password = Password;
                credentials.Credentials.UserName = UserName;
                job2.SetCredentials(credentials);
            }
        }

        private bool? JobIsDynamic
        {
            get
            {
                var isChecked = uiDynamic.IsChecked;
                return isChecked;
            }
        }

        private bool? JobIsHighPerformance
        {
            get
            {
                var isChecked = uiHighPerformance.IsChecked;
                return isChecked;
            }
        }

        private BITS.BG_JOB_PRIORITY? JobPriority
        {
            get
            {
                if (uiPriority.SelectedItem == null)
                {
                    return null;
                }
                // The Tag for each ComboBoxItem is selected to match the BITS priority values.
                var tag = (uiPriority.SelectedItem as ComboBoxItem).Tag as string;
                int value = Int32.Parse(tag);
                return (BITS.BG_JOB_PRIORITY)value;
            }
        }

        private BitsCosts? JobCosts
        {
            get
            {
                if (uiCosts.SelectedItem == null)
                {
                    return null;
                }
                // The Tag for each ComboBoxItem is already a BitsCosts enum.
                var tag = (uiCosts.SelectedItem as ComboBoxItem).Tag as BitsCosts?;
                return tag;
            }
        }

        public string UserName { get { return uiUserName.Text; } }
        public string Password { get { return uiUserName.Text; } }

        public BITS.BG_AUTH_SCHEME? AuthScheme
        {
            get
            {
                if (uiAuthScheme.SelectedItem == null)
                {
                    return null;
                }
                // The Tag for each ComboBoxItem is selected to match the BITS auth scheme values.
                var tag = (uiAuthScheme.SelectedItem as ComboBoxItem).Tag as string;
                int value = Int32.Parse(tag);
                return (BITS.BG_AUTH_SCHEME)value;
            }
        }
    }
}