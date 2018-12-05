using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// Set up the needed BITS namespaces
using BITS = BITSReference1_5;

namespace BITSManager.Tests
{
    [TestClass()]
    public class BitsConversionsTests
    {
        [TestMethod()]
        public void ConvertCostToStringTest()
        {
            Assert.AreEqual(Properties.Resources.JobCostAlways, BitsConversions.ConvertCostToString(BitsCosts.TRANSFER_ALWAYS));
            Assert.AreEqual(Properties.Resources.JobCostNotRoaming, BitsConversions.ConvertCostToString(BitsCosts.TRANSFER_NOT_ROAMING));
            Assert.AreEqual(Properties.Resources.JobCostNoSurcharge, BitsConversions.ConvertCostToString(BitsCosts.TRANSFER_NO_SURCHARGE));
            Assert.AreEqual(Properties.Resources.JobCostStandard, BitsConversions.ConvertCostToString(BitsCosts.TRANSFER_STANDARD));
            Assert.AreEqual(Properties.Resources.JobCostUnrestricted, BitsConversions.ConvertCostToString(BitsCosts.TRANSFER_UNRESTRICTED));

            // Check the individual flags
            Assert.AreEqual($"{BitsCosts.BELOW_CAP}", BitsConversions.ConvertCostToString(BitsCosts.BELOW_CAP));
            Assert.AreEqual($"{BitsCosts.BELOW_CAP}, {BitsCosts.NEAR_CAP}", BitsConversions.ConvertCostToString(BitsCosts.BELOW_CAP | BitsCosts.NEAR_CAP));

            // Check the unknown flags
            Assert.AreEqual($"11223300", BitsConversions.ConvertCostToString((BitsCosts)0x11223300));
            Assert.AreEqual($"{BitsCosts.BELOW_CAP}, 11223300", BitsConversions.ConvertCostToString(BitsCosts.BELOW_CAP | (BitsCosts)0x11223300));
        }

        [TestMethod()]
        public void ConvertJobStateToStringTest()
        {
            Assert.AreEqual(Properties.Resources.JobStateQueued, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_QUEUED));
            Assert.AreEqual(Properties.Resources.JobStateConnecting, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_CONNECTING));
            Assert.AreEqual(Properties.Resources.JobStateTransferring, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING));
            Assert.AreEqual(Properties.Resources.JobStateSuspended, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_SUSPENDED));
            Assert.AreEqual(Properties.Resources.JobStateError, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_ERROR));
            Assert.AreEqual(Properties.Resources.JobStateTransientError, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR));
            Assert.AreEqual(Properties.Resources.JobStateTransferred, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED));
            Assert.AreEqual(Properties.Resources.JobStateAcknowledged, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED));
            Assert.AreEqual(Properties.Resources.JobStateCancelled, BitsConversions.ConvertJobStateToString(BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED));

            // Validate the unknown states
            Assert.AreEqual(String.Format (Properties.Resources.JobStateUnknown, 0x11223300), BitsConversions.ConvertJobStateToString((BITS.BG_JOB_STATE)0x11223300));
        }

        [TestMethod()]
        public void ConvertJobStateToIconStringTest()
        {
            Assert.AreEqual("🙂", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_QUEUED));
            Assert.AreEqual("😵", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_CONNECTING));
            Assert.AreEqual("😏", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING));
            Assert.AreEqual("😴", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_SUSPENDED));
            Assert.AreEqual("😡", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_ERROR));
            Assert.AreEqual("😬", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR));
            Assert.AreEqual("😁", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED));
            Assert.AreEqual("😎", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED));
            Assert.AreEqual("😧", BitsConversions.ConvertJobStateToIconString(BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED));

            // Validate the unknown states
            Assert.AreEqual("11223300", BitsConversions.ConvertJobStateToIconString((BITS.BG_JOB_STATE)0x11223300));
        }

        [TestMethod()]
        public void ConvertJobTypeToStringTest()
        {
            Assert.AreEqual(Properties.Resources.JobTypeDownload, BitsConversions.ConvertJobTypeToString(BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD));
            Assert.AreEqual(Properties.Resources.JobTypeUpload, BitsConversions.ConvertJobTypeToString(BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD));
            Assert.AreEqual(Properties.Resources.JobTypeUploadReply, BitsConversions.ConvertJobTypeToString(BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD_REPLY));

            // Validate the unknown states
            Assert.AreEqual("11223300", BitsConversions.ConvertJobTypeToString((BITS.BG_JOB_TYPE)0x11223300));
        }

        [TestMethod()]
        public void ConvertPriorityToStringTest()
        {
            Assert.AreEqual(Properties.Resources.JobPriorityForeground, BitsConversions.ConvertJobPriorityToString(BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_FOREGROUND));
            Assert.AreEqual(Properties.Resources.JobPriorityHigh, BitsConversions.ConvertJobPriorityToString(BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_HIGH));
            Assert.AreEqual(Properties.Resources.JobPriorityLow, BitsConversions.ConvertJobPriorityToString(BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_LOW));
            Assert.AreEqual(Properties.Resources.JobPriorityNormal, BitsConversions.ConvertJobPriorityToString(BITS.BG_JOB_PRIORITY.BG_JOB_PRIORITY_NORMAL));

            // Validate the unknown states
            Assert.AreEqual("11223300", BitsConversions.ConvertJobPriorityToString((BITS.BG_JOB_PRIORITY)0x11223300));
        }
    }
}