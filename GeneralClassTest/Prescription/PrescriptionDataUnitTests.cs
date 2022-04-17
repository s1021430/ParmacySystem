using System;
using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalOrders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GeneralClassTest.Prescription
{
    [TestClass]
    public class PrescriptionDataUnitTests
    {
        [TestMethod]
        public void Test_處方_計算申報點數_藥品點數等於所有非自費藥品總點數和()
        {
            var stub = Substitute.For<PrescriptionData>();
            stub.MedicalOrders = new List<MedicalOrder>
            {
                GetFakeOrder(MedicalOrderType.Medicine, false, 10),
                GetFakeOrder(MedicalOrderType.Medicine, false, 5),
                GetFakeOrder(MedicalOrderType.Medicine, true, 7),
                GetFakeOrder(MedicalOrderType.SpecialMaterial, false, 5)
            };
            stub.CalculateMedicinePoint();
            Assert.AreEqual(15, stub.MedicinePoint);
        }

        [TestMethod]
        public void Test_處方_計算申報點數_申報點數點數等於總點數減去部分負擔()
        {
            var stub = Substitute.For<PrescriptionData>();
            stub.TotalPoint = 100;
            stub.CopaymentPoint = 20;
            stub.CalculateDeclarePoint();
            Assert.AreEqual(80, stub.DeclarePoint);
        }

        [TestMethod]
        public void Test_處方_計算申報點數_總點數等於藥品點_特材點_藥事服務點_部分負擔點總和()
        {
            var stub = Substitute.For<PrescriptionData>();
            stub.MedicinePoint = 200;
            stub.SpecialMaterialPoint = 100;
            stub.MedicalServicePoint = 64;
            stub.CopaymentPoint = 20;
            stub.CalculateTotalPoint();
            Assert.AreEqual(384, stub.TotalPoint);
        }

        [TestMethod]
        public void Test_處方_確實新增藥事服務醫令()
        {
            var stub = Substitute.For<PrescriptionData>();
            stub.MedicineDays = 6;
            stub.AdjustDate = DateTime.Today;
            stub.CreateMedicalServiceOrder();
            Assert.IsTrue(stub.MedicalOrders.Count(o => o.Type is MedicalOrderType.MedicalService) == 1);
        }

        private static MedicalOrder GetFakeOrder(MedicalOrderType type, bool ownExpense, int totalPoint)
        {
            MedicalOrder fakeOrder;
            switch (type)
            {
                case MedicalOrderType.Medicine:
                    fakeOrder = Substitute.For<MedicineOrder>();
                    fakeOrder.OwnExpense = ownExpense;
                    fakeOrder.TotalPoint = totalPoint;
                    return fakeOrder;
                
                case MedicalOrderType.SpecialMaterial:
                    fakeOrder = Substitute.For<SpecialMaterialOrder>();
                    fakeOrder.OwnExpense = ownExpense;
                    fakeOrder.TotalPoint = totalPoint;
                    return fakeOrder;
                case MedicalOrderType.NoCharged:
                case MedicalOrderType.SpecialMaterialNoSubsidy:
                case MedicalOrderType.SpecialMaterialNotInPaymentRule:
                case MedicalOrderType.Virtual:
                case MedicalOrderType.Null:
                default:
                    return null;
            }
        }
    }
}
