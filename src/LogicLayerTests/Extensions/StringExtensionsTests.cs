using LogicLayer.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void CleanOrdinals_TestsVariousCases()
        {
            string v1 = "1. Ex".CleanOrdinals();
            string v2 = "1) Ex".CleanOrdinals();
            string v3 = "1)Ex".CleanOrdinals();
            string v4 = "1 Ex".CleanOrdinals();

            Assert.AreEqual(v1, "Ex");
            Assert.AreEqual(v2, "Ex");
            Assert.AreEqual(v3, "Ex");
            Assert.AreEqual(v4, "Ex");
        }

        [TestMethod]
        public void CleanQuotationMarks_TestsVariousCases()
        {
            string v1 = "\"Ex\"".CleanQuotationMarks(); // "Ex"
            string v2 = "\" Ex \"".CleanQuotationMarks(); // " Ex "
            string v3 = "'Ex'".CleanQuotationMarks();
            string v4 = "' Ex '".CleanQuotationMarks();

            Assert.AreEqual(v1, "Ex");
            Assert.AreEqual(v2, " Ex ");
            Assert.AreEqual(v3, "Ex");
            Assert.AreEqual(v4, " Ex ");
        }

    }
}