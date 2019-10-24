using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class SenseRegisterPoliceTests
    {
        [TestMethod]
        public void IsSenseRegister_InputIsSenseRegister_ShouldReturnTrue()
        {
            bool result = SenseRegisterPolice.IsSenseRegister("Dated");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSenseRegister_InputIsSenseRegister2_ShouldReturnTrue()
        {
            bool result = SenseRegisterPolice.IsSenseRegister("dated");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSenseRegister_InputIsNOTSenseRegister_ShouldReturnTrue()
        {
            bool result = SenseRegisterPolice.IsSenseRegister("American");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSenseRegister_InputIsNOTSenseRegister2_ShouldReturnTrue()
        {
            bool result = SenseRegisterPolice.IsSenseRegister("american");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSenseRegister_InputIsEmpty_ShouldReturnTrue()
        {
            bool result = SenseRegisterPolice.IsSenseRegister("");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSenseRegister_InputIsNull_ShouldReturnTrue()
        {
            bool result = SenseRegisterPolice.IsSenseRegister(null);

            Assert.IsFalse(result);
        }
    }
}
