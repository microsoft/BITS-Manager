// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows.Controls;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;
using BITS4 = BITSReference4_0;
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
            var job2 = (BITS.IBackgroundCopyJob2)job; // Job2 exists on all supported version of Windows.
            var job5 = job as BITS5.IBackgroundCopyJob5; // Job5 will be null on, e.g., Windows 7 and earlier.
            var jobHttpOptions = job as BITS4.IBackgroundCopyJobHttpOptions;

            if (job5 != null)
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

            if (jobHttpOptions != null)
            {
                var text = _uiCustomHeadersAll.Text;
                if (!String.IsNullOrWhiteSpace(text))
                {
                    jobHttpOptions.SetCustomHeaders(text);
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
                var serverCredentials = new BITS.BG_AUTH_CREDENTIALS();
                serverCredentials.Scheme = (BITS.BG_AUTH_SCHEME)authScheme.Value;
                serverCredentials.Target = BITS.BG_AUTH_TARGET.BG_AUTH_TARGET_SERVER;
                serverCredentials.Credentials.Password = _uiPassword.Text;
                serverCredentials.Credentials.UserName = _uiUserName.Text;
                job2.SetCredentials(serverCredentials);
            }

            // Some enterprises have a proxy that requires implicit credentials. For
            // those places, allow the user to add the NEGOTIAGE and NTLM schemes.
            // NEGOTIATE is newer and is more common.
            if (_jobIsAuthProxyImplicit.HasValue && _jobIsAuthProxyImplicit.Value)
            {
                var proxyCredentials = new BITS.BG_AUTH_CREDENTIALS();
                proxyCredentials.Target = BITS.BG_AUTH_TARGET.BG_AUTH_TARGET_PROXY;
                proxyCredentials.Scheme = BITS.BG_AUTH_SCHEME.BG_AUTH_SCHEME_NEGOTIATE;
                proxyCredentials.Credentials.Password = null;
                proxyCredentials.Credentials.UserName = null;
                job2.SetCredentials(proxyCredentials);

                // Some enterprises won't have Nego set up; for them, also allow
                // plain NTLM without the possibility of Kerberos.
                proxyCredentials.Scheme = BITS.BG_AUTH_SCHEME.BG_AUTH_SCHEME_NTLM;
                job2.SetCredentials(proxyCredentials);
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

        private bool? _jobIsAuthProxyImplicit
        {
            get
            {
                var isChecked = _uiAuthProxyImplicit.IsChecked;
                return isChecked;
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

        private void OnClearCustomHeaders(object sender, System.Windows.RoutedEventArgs e)
        {
            _uiCustomHeadersAll.Text = "";
        }

        private void OnAddNewCustomHeader(object sender, System.Windows.RoutedEventArgs e)
        {
            var newHeader = _uiNewCustomHeader.Text + "\r\n";
            _uiCustomHeadersAll.Text += newHeader;
        }
    }
}
