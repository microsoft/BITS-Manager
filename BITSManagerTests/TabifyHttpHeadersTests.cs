// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BITSManager.Tests
{
    [TestClass()]
    public class TabifyHttpHeadersTests
    {
        const string TAB = "        ";
        [TestMethod()]
        public void AddTabsTest()
        {
            Assert.AreEqual("HEADER: " + TAB + TAB + "VALUE:VALUE2", TabifyHttpHeaders.AddTabs("HEADER:VALUE:VALUE2"));
            Assert.AreEqual("HEADER: " + TAB + TAB + "VALUE:VALUE2\r\n\theader: " + TAB + TAB + "value:value2",
                TabifyHttpHeaders.AddTabs("HEADER:VALUE:VALUE2\r\nheader:value:value2"));

            // Try some unusual values
            Assert.AreEqual("", TabifyHttpHeaders.AddTabs(""));
            Assert.AreEqual("HEADER", TabifyHttpHeaders.AddTabs("HEADER"));
            Assert.AreEqual(":" + "       " + TAB + TAB + "VALUE", TabifyHttpHeaders.AddTabs(":VALUE"));
        }

        [TestMethod()]
        public void GetAlignedSpacesTest()
        {
            Assert.AreEqual(TAB + TAB + TAB, TabifyHttpHeaders.GetAlignedSpaces(0));
            Assert.AreEqual("       " + TAB + TAB, TabifyHttpHeaders.GetAlignedSpaces(1));
            Assert.AreEqual("      " + TAB + TAB, TabifyHttpHeaders.GetAlignedSpaces(2));
            Assert.AreEqual("     " + TAB + TAB, TabifyHttpHeaders.GetAlignedSpaces(3));

            // Test values near the TabSize (8) even though that shouldn't
            // make a different for what we're doing
            Assert.AreEqual(" " + TAB + TAB, TabifyHttpHeaders.GetAlignedSpaces(7));
            Assert.AreEqual(TAB + TAB, TabifyHttpHeaders.GetAlignedSpaces(8));
            Assert.AreEqual("       " + TAB, TabifyHttpHeaders.GetAlignedSpaces(9));

            // Test values around the TargetLength value
            Assert.AreEqual(" ", TabifyHttpHeaders.GetAlignedSpaces(23));
            Assert.AreEqual(TAB, TabifyHttpHeaders.GetAlignedSpaces(24));
            Assert.AreEqual("       ", TabifyHttpHeaders.GetAlignedSpaces(25));

            // Test large values
            Assert.AreEqual(TAB, TabifyHttpHeaders.GetAlignedSpaces(80));
            Assert.AreEqual("       ", TabifyHttpHeaders.GetAlignedSpaces(81));

            // Test very large values
            Assert.AreEqual("     ", TabifyHttpHeaders.GetAlignedSpaces(2000000 * 8 + 3));

            // Test negative values
            Assert.AreEqual(" ", TabifyHttpHeaders.GetAlignedSpaces(-1));
            Assert.AreEqual(" ", TabifyHttpHeaders.GetAlignedSpaces(-99999999));
        }
    }
}
