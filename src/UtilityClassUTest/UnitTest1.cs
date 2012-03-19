using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UtilityClass;
namespace UtilityClassUTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(RedisXmlReader.Instance.host, "localhost");
            Assert.AreEqual(RedisXmlReader.Instance.port, 6379);
        }
    }
}
