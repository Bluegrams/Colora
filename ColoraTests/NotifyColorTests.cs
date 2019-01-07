using Microsoft.VisualStudio.TestTools.UnitTesting;
using Colora;
using System;

namespace Colora.Tests
{
    [TestClass]
    public class NotifyColorTests
    {
        [TestMethod]
        public void HSBPropertiesTest()
        {
            NotifyColor nfcol = new NotifyColor(231, 32, 78);
            Assert.AreEqual(346, nfcol.Hue);
            Assert.AreEqual(86, nfcol.SatHSB);
            Assert.AreEqual(91, nfcol.Bright);
        }

        [TestMethod]
        public void HSLPropertiesTest()
        {
            NotifyColor nfcol = new NotifyColor(43, 241, 28);
            Assert.AreEqual(116, nfcol.Hue);
            Assert.AreEqual(88, nfcol.SatHSL);
            Assert.AreEqual(53, nfcol.Light);
        }

        [TestMethod]
        public void SetFromHexTest()
        {
            NotifyColor nfcol = new NotifyColor(0, 0, 0);
            nfcol.SetFromHex("#174CBB");
            Assert.AreEqual(23, nfcol.Red);
            Assert.AreEqual(76, nfcol.Green);
            Assert.AreEqual(187, nfcol.Blue);
        }

        [TestMethod]
        public void SetFromHSLTest()
        {
            NotifyColor nfcol = new NotifyColor(0, 0, 0);
            nfcol.SetFromHSL(37, 0.95, 0.42);
            Assert.AreEqual(209, nfcol.Red);
            Assert.AreEqual(131, nfcol.Green);
            Assert.AreEqual(5, nfcol.Blue);
        }

        [TestMethod]
        public void SetFromHSBTest()
        {
            NotifyColor nfcol = new NotifyColor(0, 0, 0);
            nfcol.SetFromHSB(312, 0.67, 0.43);
            Assert.AreEqual(110, nfcol.Red);
            Assert.AreEqual(36, nfcol.Green);
            Assert.AreEqual(95, nfcol.Blue);
        }

        [TestMethod]
        public void SetFromCMYKTest()
        {
            NotifyColor nfcol = new NotifyColor(0, 0, 0);
            nfcol.SetFromCMYK(0.69, 0.26, 0.52, 0.18);
            Assert.AreEqual(65, nfcol.Red);
            Assert.AreEqual(155, nfcol.Green);
            Assert.AreEqual(100, nfcol.Blue);
        }
    }
}