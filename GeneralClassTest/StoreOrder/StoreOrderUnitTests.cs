using GeneralClass.Employee.EntityIndex;
using GeneralClass.PurchaseOrder;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseOrder.SearchCondition;
using GeneralClass.PurchaseRequisition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GeneralClassTest.StoreOrder
{
    [TestClass]
    public class StoreOrderUnitTests
    {
        [TestMethod]
        public void Test_進貨單帶入採購單後資料有確實複製()
        {
            var mockPurchaseOrder = Substitute.For<PurchaseOrder>();
            var stubPurchaseRequisition = Substitute.For<PurchaseRequisition>();

            mockPurchaseOrder.BringIn(stubPurchaseRequisition);

            Assert.AreEqual(mockPurchaseOrder.WareHouseID, stubPurchaseRequisition.WareHouseID);
            Assert.AreEqual(mockPurchaseOrder.RequisitionID, stubPurchaseRequisition.ID);
            Assert.AreEqual(mockPurchaseOrder.ManufactoryID, stubPurchaseRequisition.ManufactoryID);
            Assert.AreEqual(mockPurchaseOrder.Note, stubPurchaseRequisition.Note);
            Assert.AreEqual(mockPurchaseOrder.Products.Count, stubPurchaseRequisition.Products.Count);
        }

        [TestMethod]
        public void Test_進貨單完成後狀態成功改變()
        {
            var mockPurchaseOrder = Substitute.For<PurchaseOrder>();

            mockPurchaseOrder.Complete(new EmployeeID(1));

            Assert.AreEqual(mockPurchaseOrder.Status, DataStatus.Complete);
        }

        [TestMethod]
        public void Test_進貨單搜尋條件所有為空時的判斷式()
        {
            var condition = new PurchaseOrderSearchCondition();

            Assert.AreEqual(false, condition.HasCondition());
        }

        [TestMethod]
        public void Test_進貨單搜尋條件有值時的判斷式()
        {
            var condition = new PurchaseOrderSearchCondition( id: new PurchaseOrderID("Test") );

            Assert.AreEqual(true, condition.HasCondition());
        }
    }
}
