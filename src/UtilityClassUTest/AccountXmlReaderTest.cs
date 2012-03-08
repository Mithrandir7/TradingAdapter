using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountClass;
namespace UtilityClassUTest
{
    [TestClass]
    public class AccountXmlReaderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(2, AccountXmlReader.Instance.getAccountList().Count);
        }
    }
}
