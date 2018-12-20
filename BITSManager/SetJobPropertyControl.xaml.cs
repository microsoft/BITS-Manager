// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows.Controls;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;
using BITS5 = BITSReference5_0;

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
                var costs = _jobCosts;
                if (costs.HasValue)
                {
                    var value = new BITS5.BITS_JOB_PROPERTY_VALUE();
                    value.Dword = (UInt32)costs.Value;
                    job5.SetProperty(BITS5.BITS_JOB_PROPERTY_ID.BITS_JOB_PROPERTY_ID_COST_FLAGS, value);
                }

                var isDynamic = _jobIsDynamic;
                if (isDynamic.HasValue)
                {
                    var value = new BITS5.BITS_JOB_PROPERTY_VALUE();
                    value.Enable = isDynamic.Value ? 1 : 0;
                    job5.SetProperty(BITS5.BITS_JOB_PROPERTY_ID.BITS_JOB_PROPERTY_DYNAMIC_CONTENT, value);
                }

                var isHighPerformance = _jobIsHighPerformance;
                if (isHighPerformance.HasValue)
                {
                    var value = new BITS5.BITS_JOB_PROPERTY_VALUE();
                    value.Enable = isHighPerformance.Value ? 1 : 0;
                    job5.SetProperty(BITS5.BITS_JOB_PROPERTY_ID.BITS_JOB_PROPERTY_HIGH_PERFORMANCE, value);
                }
            }

            var priority = _jobPriority;
            if (priority.HasValue)
            {
                job.SetPriority(priority.Value);
            }

            var authScheme = _authScheme;
            if (authScheme.HasValue)
            {
                var job2 = (BITS.IBackgroundCopyJob2)job; // Job2 exists on all supported version of Windows.
                var credentials = new BITS.BG_AUTH_CREDENTIALS();
                credentials.Scheme = (BITS.BG_AUTH_SCHEME)authScheme.Value;
                credentials.Target = BITS.BG_AUTH_TARGET.BG_AUTH_TARGET_SERVER;
                // This app doesn't support setting proxy auth.
                credentials.Credentials.Password = _uiPassword.Text;
                credentials.Credentials.UserName = _uiUserName.Text;
                job2.SetCredentials(credentials);
            }
        }

        private BITS.BG_AUTH_SCHEME? _authScheme
        {
            get
            {
                if (_uiAuthScheme.SelectedItem == null)
                {
                    return null;
                }
                // The Tag for each ComboBoxItem is selected to match the BITS auth scheme values.
                var tag = (_uiAuthScheme.SelectedItem as ComboBoxItem).Tag as string;
                int value = Int32.Parse(tag);
                return (BITS.BG_AUTH_SCHEME)value;
            }
        }

        private BitsCosts? _jobCosts
        {
            get
            {
                if (_uiCosts.SelectedItem == null)
                {
                    return null;
                }
                // The Tag for each ComboBoxItem is already a BitsCosts enum.
                var tag = (_uiCosts.SelectedItem as ComboBoxItem).Tag as BitsCosts?;
                return tag;
            }
        }

        private bool? _jobIsDynamic
        {
            get
            {
                var isChecked = _uiDynamic.IsChecked;
                return isChecked;
            }
        }

        private bool? _jobIsHighPerformance
        {
            get
            {
                var isChecked = _uiHighPerformance.IsChecked;
                return isChecked;
            }
        }

        private BITS.BG_JOB_PRIORITY? _jobPriority
        {
            get
            {
                if (_uiPriority.SelectedItem == null)
                {
                    return null;
                }
                // The Tag for each ComboBoxItem is selected to match the BITS priority values.
                var tag = (_uiPriority.SelectedItem as ComboBoxItem).Tag as string;
                int value = Int32.Parse(tag);
                return (BITS.BG_JOB_PRIORITY)value;
            }
        }
    }
}
