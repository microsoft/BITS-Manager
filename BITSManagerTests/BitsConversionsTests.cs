using BITSManager;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual("11223300", BitsConversions.ConvertCostToString((BitsCosts)0x11223300));
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
            Assert.AreEqual(String.Format(Properties.Resources.JobStateUnknown, 0x11223300), BitsConversions.ConvertJobStateToString((BITS.BG_JOB_STATE)0x11223300));
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

        [TestMethod()]
        public void ToGuidTest()
        {
            BITS.GUID bitsGuid = MakeGuid (0xF0000001, 0xF002, 0xF003);
            Guid dotNetGuid = bitsGuid.ToGuid();
            string text = dotNetGuid.ToString();

            Assert.AreEqual("f0000001-f002-f003-0001-020304050607", text);
        }

        private BITS.GUID MakeGuid(uint data1, ushort data2, ushort data3, byte b0=0, byte b1=1, byte b2=2, byte b3=3, byte b4=4, byte b5=5, byte b6=6, byte b7=7 )
        {
            BITS.GUID bitsGuid = new BITS.GUID();
            bitsGuid.Data1 = data1;
            bitsGuid.Data2 = data2;
            bitsGuid.Data3 = data3;
            bitsGuid.Data4 = new byte[8] { b0, b1, b2, b3, b4, b5, b6, b7 };
            return bitsGuid;
        }

        [TestMethod()]
        public void GuidEqualsTest()
        {
            var a1 = MakeGuid(0xF00001, 0xF002, 0xF003);
            var a2 = MakeGuid(0xF00001, 0xF002, 0xF003);
            var b = MakeGuid(2, 2, 2);

            Assert.AreEqual (true, a1.GuidEquals (a2));
            Assert.AreEqual (true, a1.GuidEquals (a1));
            Assert.AreEqual (false, a1.GuidEquals (b));
        }
    }
}
