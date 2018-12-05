using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BITSManager.Tests
{
    [TestClass()]
    public class TabifyHttpHeadersTests
    {
        [TestMethod()]
        public void AddTabsTest()
        {
            const string TAB = "        ";
            Assert.AreEqual("HEADER: " + TAB + TAB + "VALUE:VALUE2", TabifyHttpHeaders.AddTabs("HEADER:VALUE:VALUE2"));
            Assert.AreEqual("HEADER: " + TAB + TAB + "VALUE:VALUE2\r\n\theader: " + TAB + TAB + "value:value2",
                TabifyHttpHeaders.AddTabs("HEADER:VALUE:VALUE2\r\nheader:value:value2"));

            // Try some unusual values
            Assert.AreEqual("", TabifyHttpHeaders.AddTabs(""));
            Assert.AreEqual("HEADER", TabifyHttpHeaders.AddTabs("HEADER"));
            Assert.AreEqual(":" + "       " + TAB + TAB + "VALUE", TabifyHttpHeaders.AddTabs(":VALUE"));
        }

        [TestMethod()]
        public void GetTabsTest()
        {
            const string TAB = "        ";
            Assert.AreEqual(TAB + TAB + TAB, TabifyHttpHeaders.GetTabs(0));
            Assert.AreEqual("       " + TAB + TAB, TabifyHttpHeaders.GetTabs(1));
            Assert.AreEqual("      " + TAB + TAB, TabifyHttpHeaders.GetTabs(2));
            Assert.AreEqual("     " + TAB + TAB, TabifyHttpHeaders.GetTabs(3));

            // Test values near the TabSize (8) even though that shouldn't
            // make a different for what we're doing
            Assert.AreEqual(" " + TAB + TAB, TabifyHttpHeaders.GetTabs(7));
            Assert.AreEqual(TAB + TAB, TabifyHttpHeaders.GetTabs(8));
            Assert.AreEqual("       " + TAB, TabifyHttpHeaders.GetTabs(9));

            // Test values around the TargetLength value
            Assert.AreEqual(" ", TabifyHttpHeaders.GetTabs(23));
            Assert.AreEqual(TAB, TabifyHttpHeaders.GetTabs(24));
            Assert.AreEqual("       ", TabifyHttpHeaders.GetTabs(25));

            // Test large values
            Assert.AreEqual(TAB, TabifyHttpHeaders.GetTabs(80));
            Assert.AreEqual("       ", TabifyHttpHeaders.GetTabs(81));

            // Test very large values
            Assert.AreEqual("     ", TabifyHttpHeaders.GetTabs(2000000 * 8 + 3));

            // Test negative values
            Assert.AreEqual(" ", TabifyHttpHeaders.GetTabs(-1));
            Assert.AreEqual(" ", TabifyHttpHeaders.GetTabs(-99999999));
        }
    }
}