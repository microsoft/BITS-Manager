// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Text;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;

namespace BITSManager
{
    // Must recreate the BITS COST values; they don't transfer over when converting the IDL file.
    // From bits5_0.idl.
    // The transfer policy is a two-step conversion: there are 5 documented high-level values
    // (e.g., BITS_COST_STATE_TRANSFER_NOT_ROAMING) which are composed from a series of
    // low-level values (e.g., BITS_COST_STATE_UNRESTRICTED)
    // #define BITS_COST_STATE_UNRESTRICTED          0x1
    // #define BITS_COST_STATE_CAPPED_USAGE_UNKNOWN  0x2
    // #define BITS_COST_STATE_BELOW_CAP             0x4
    // #define BITS_COST_STATE_NEAR_CAP              0x8
    // #define BITS_COST_STATE_OVERCAP_CHARGED       0x10
    // #define BITS_COST_STATE_OVERCAP_THROTTLED     0x20
    // #define BITS_COST_STATE_USAGE_BASED           0x40
    // #define BITS_COST_STATE_ROAMING               0x80
    // #define BITS_COST_OPTION_IGNORE_CONGESTION    0x80000000
    // #define BITS_COST_STATE_RESERVED              0x40000000

    // #define BITS_COST_STATE_TRANSFER_NOT_ROAMING  (BITS_COST_OPTION_IGNORE_CONGESTION|BITS_COST_STATE_USAGE_BASED|BITS_COST_STATE_OVERCAP_THROTTLED|BITS_COST_STATE_OVERCAP_CHARGED|BITS_COST_STATE_NEAR_CAP|BITS_COST_STATE_BELOW_CAP|BITS_COST_STATE_CAPPED_USAGE_UNKNOWN|BITS_COST_STATE_UNRESTRICTED)
    // #define BITS_COST_STATE_TRANSFER_NO_SURCHARGE (BITS_COST_OPTION_IGNORE_CONGESTION|BITS_COST_STATE_USAGE_BASED|BITS_COST_STATE_OVERCAP_THROTTLED|BITS_COST_STATE_NEAR_CAP|BITS_COST_STATE_BELOW_CAP|BITS_COST_STATE_CAPPED_USAGE_UNKNOWN|BITS_COST_STATE_UNRESTRICTED)
    // #define BITS_COST_STATE_TRANSFER_STANDARD     (BITS_COST_OPTION_IGNORE_CONGESTION|BITS_COST_STATE_USAGE_BASED|BITS_COST_STATE_OVERCAP_THROTTLED|BITS_COST_STATE_BELOW_CAP|BITS_COST_STATE_CAPPED_USAGE_UNKNOWN|BITS_COST_STATE_UNRESTRICTED)
    // #define BITS_COST_STATE_TRANSFER_UNRESTRICTED (BITS_COST_OPTION_IGNORE_CONGESTION|BITS_COST_STATE_OVERCAP_THROTTLED|BITS_COST_STATE_UNRESTRICTED)
    // #define BITS_COST_STATE_TRANSFER_ALWAYS       (BITS_COST_OPTION_IGNORE_CONGESTION|BITS_COST_STATE_ROAMING|BITS_COST_STATE_USAGE_BASED|BITS_COST_STATE_OVERCAP_THROTTLED|BITS_COST_STATE_OVERCAP_CHARGED|BITS_COST_STATE_NEAR_CAP|BITS_COST_STATE_BELOW_CAP|BITS_COST_STATE_CAPPED_USAGE_UNKNOWN|BITS_COST_STATE_UNRESTRICTED)

    public enum BitsCosts : UInt32
    {
        // See https://msdn.microsoft.com/library/9dc2c020-06c0-41dd-bf36-203432ad9d4f for a full discussion
        // of how network metering is measures.
        UNRESTRICTED = 0x1,

        CAPPED_USAGE_UNKNOWN = 0x2,
        BELOW_CAP = 0x4,
        NEAR_CAP = 0x8,
        OVERCAP_CHARGED = 0x10,
        OVERCAP_THROTTLED = 0x20,
        USAGE_BASED = 0x40,
        ROAMING = 0x80,
        IGNORE_CONGESTION = 0x80000000,

        TRANSFER_NOT_ROAMING = (IGNORE_CONGESTION | USAGE_BASED | OVERCAP_THROTTLED | OVERCAP_CHARGED | NEAR_CAP | BELOW_CAP | CAPPED_USAGE_UNKNOWN | UNRESTRICTED),
        TRANSFER_NO_SURCHARGE = (IGNORE_CONGESTION | USAGE_BASED | OVERCAP_THROTTLED | NEAR_CAP | BELOW_CAP | CAPPED_USAGE_UNKNOWN | UNRESTRICTED),
        TRANSFER_STANDARD = (IGNORE_CONGESTION | USAGE_BASED | OVERCAP_THROTTLED | BELOW_CAP | CAPPED_USAGE_UNKNOWN | UNRESTRICTED),
        TRANSFER_UNRESTRICTED = (IGNORE_CONGESTION | OVERCAP_THROTTLED | UNRESTRICTED),
        TRANSFER_ALWAYS = (IGNORE_CONGESTION | ROAMING | USAGE_BASED | OVERCAP_THROTTLED | OVERCAP_CHARGED | NEAR_CAP | BELOW_CAP | CAPPED_USAGE_UNKNOWN | UNRESTRICTED),
    }

    public class BitsConversions
    {
        /// <summary>
        ///  Converts the cost dword (uint) into a string. This is non-trivial because the cost values
        ///  aren't translated into C# enum values.
        /// </summary>
        /// <param name="cost"></param>
        /// <returns></returns>
        public static string ConvertCostToString(BitsCosts cost)
        {
            if (cost == BitsCosts.TRANSFER_NOT_ROAMING)
            {
                return Properties.Resources.JobCostNotRoaming;
            }
            if (cost == BitsCosts.TRANSFER_NO_SURCHARGE)
            {
                return Properties.Resources.JobCostNoSurcharge;
            }
            if (cost == BitsCosts.TRANSFER_STANDARD)
            {
                return Properties.Resources.JobCostStandard;
            }
            if (cost == BitsCosts.TRANSFER_UNRESTRICTED)
            {
                return Properties.Resources.JobCostUnrestricted;
            }
            if (cost == BitsCosts.TRANSFER_ALWAYS)
            {
                return Properties.Resources.JobCostAlways;
            }

            // It wasn't one of the standard sets. Break it into the known cost values plus "all the rest"
            // Because these values are never encounted in real life, it's OK to use the ENUM value
            // as the output value instead of a translated string.
            var costBuilder = new StringBuilder();
            List<BitsCosts> allCostsToCheck = new List<BitsCosts>() {
                BitsCosts.UNRESTRICTED,
                BitsCosts.CAPPED_USAGE_UNKNOWN,
                BitsCosts.BELOW_CAP,
                BitsCosts.NEAR_CAP,
                BitsCosts.OVERCAP_CHARGED,
                BitsCosts.OVERCAP_THROTTLED,
                BitsCosts.USAGE_BASED,
                BitsCosts.ROAMING,
                BitsCosts.IGNORE_CONGESTION};

            foreach (var item in allCostsToCheck)
            {
                if ((cost & item) != 0)
                {
                    if (costBuilder.Length > 0)
                    {
                        costBuilder.Append(", ");
                    }
                    costBuilder.Append(item.ToString());
                    cost &= ~item;
                }
            }
            // Handle values that we don't know about ahead of time. These will
            // be printed as a simple hex value.
            if (cost != 0)
            {
                if (costBuilder.Length > 0)
                {
                    costBuilder.Append(", ");
                }
                costBuilder.Append(string.Format("{0:X}", cost));
            }
            return costBuilder.ToString();
        }

        public static string ConvertJobStateToString(BITS.BG_JOB_STATE jobState)
        {
            switch (jobState)
            {
                case BITS.BG_JOB_STATE.BG_JOB_STATE_QUEUED: return Properties.Resources.JobStateQueued;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_CONNECTING: return Properties.Resources.JobStateConnecting;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING: return Properties.Resources.JobStateTransferring;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_SUSPENDED: return Properties.Resources.JobStateSuspended;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_ERROR: return Properties.Resources.JobStateError;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR: return Properties.Resources.JobStateTransientError;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED: return Properties.Resources.JobStateTransferred;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED: return Properties.Resources.JobStateAcknowledged;
                case BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED: return Properties.Resources.JobStateCancelled;
                default: return String.Format(Properties.Resources.JobStateUnknown, (int)jobState);
            }
        }

        public static string ConvertJobStateToIconString(BITS.BG_JOB_STATE jobState)
        {
            switch (jobState)
            {
                case BITS.BG_JOB_STATE.BG_JOB_STATE_QUEUED: return "🙂";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_CONNECTING: return "😵";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING: return "😏";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_SUSPENDED: return "😴";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_ERROR: return "😡";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR: return "😬";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED: return "😁";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED: return "😎";
                case BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED: return "😧";
                default: return String.Format ("{0:X}", jobState);
            }
        }

        public static string ConvertJobTypeToString(BITS.BG_JOB_TYPE jobType)
        {
            switch (jobType)
            {
                case BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD: return Properties.Resources.JobTypeDownload;
                case BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD: return Properties.Resources.JobTypeUpload;
                case BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD_REPLY: return Properties.Resources.JobTypeUploadReply;
                default: return String.Format("{0:X}", jobType);
            }
        }

        public static string ConvertJobPriorityToString(BITS.BG_JOB_PRIORITY jobPriority)
        {
            switch (jobPriority)
            {
                case BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_FOREGROUND: return Properties.Resources.JobPriorityForeground;
                case BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_HIGH: return Properties.Resources.JobPriorityHigh;
                case BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_LOW: return Properties.Resources.JobPriorityLow;
                case BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_NORMAL: return Properties.Resources.JobPriorityNormal;
                default: return String.Format("{0:X}", jobPriority);
            }
        }
    }
}