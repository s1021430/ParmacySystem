using System;
using GeneralClass.Prescription.MedicalOrders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneralClassTest.MedicalOrders
{
    [TestClass]
    public class MedicalOrderUnitTest
    {
        [TestMethod]
        public void Test_藥事服務_小於7天()
        {
            const string 小於7天藥事服務費代碼 = "05202B";
            const int 小於7天藥事服務費點數 = 54;
            var adjustDate = DateTime.Today;
            var stub = new MedicalServiceOrder(6, adjustDate);
            stub.CountTotalPoint();
            Assert.AreEqual(小於7天藥事服務費代碼, stub.ID.ID);
            Assert.AreEqual(小於7天藥事服務費點數, stub.TotalPoint);
        }

        [TestMethod]
        public void Test_藥事服務_介於7到13天()
        {
            const string 介於7到13天藥事服務費代碼 = "05223B";
            const int 介於7到13天藥事服務費點數 = 54;
            var adjustDate = DateTime.Today;
            var stub = new MedicalServiceOrder(7, adjustDate);
            stub.CountTotalPoint();
            Assert.AreEqual(介於7到13天藥事服務費代碼, stub.ID.ID);
            Assert.AreEqual(介於7到13天藥事服務費點數, stub.TotalPoint);
        }

        [TestMethod]
        public void Test_藥事服務_介於14到27天()
        {
            const string 介於14到27天藥事服務費代碼 = "05206B";
            const int 介於14到27天藥事服務費點數 = 65;
            var adjustDate = DateTime.Today;
            var stub = new MedicalServiceOrder(14, adjustDate);
            stub.CountTotalPoint();
            Assert.AreEqual(介於14到27天藥事服務費代碼, stub.ID.ID);
            Assert.AreEqual(介於14到27天藥事服務費點數, stub.TotalPoint);
        }

        [TestMethod]
        public void Test_藥事服務_超過28天()
        {
            const string 超過28天 = "05234D";
            const int 超過天藥事服務費點數 = 75;
            var adjustDate = DateTime.Today;
            var stub = new MedicalServiceOrder(30, adjustDate);
            stub.CountTotalPoint();
            Assert.AreEqual(超過28天, stub.ID.ID);
            Assert.AreEqual(超過天藥事服務費點數, stub.TotalPoint);
        }
    }
}
