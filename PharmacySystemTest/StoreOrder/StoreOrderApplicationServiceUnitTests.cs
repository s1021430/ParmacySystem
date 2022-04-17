using GeneralClass.Customer;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.PurchaseOrder;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseOrder.Validator;
using GeneralClass.PurchaseRequisition;
using GeneralClass.WareHouse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystemTest.StoreOrder
{
    [TestClass]
    public class StoreOrderApplicationServiceUnitTests
    {
        [TestMethod]
        public void Test_應用層建立進貨單_預設值檢查()
        {
            var newOrderID = "1";

            var stubPurchaseOrderRepository = Substitute.For<IPurchaseOrderRepository>();
            stubPurchaseOrderRepository.GetNewPurchaseOrderID().Returns(new PurchaseOrderID(newOrderID));
            stubPurchaseOrderRepository.Create(Arg.Any<PurchaseOrder>()).Returns(true);

            var stubPurchaseRequisitionRepository = Substitute.For<IPurchaseRequisitionRepository>();
            var stubCustomerRepository = Substitute.For<ICustomerRepository>();
            var stubWareHouseRepository = Substitute.For<IWareHouseRepository>();
            
            var stubValidator = Substitute.For<IStoreOrderValidator>();

            var service = new PurchaseOrderApplicationService(stubPurchaseOrderRepository, stubPurchaseRequisitionRepository, stubWareHouseRepository, stubCustomerRepository, stubValidator);

            var newOrder = service.CreatePurchaseOrder(new EmployeeID(1));

            Assert.AreEqual(newOrderID, newOrder.ID);
        }

        [TestMethod]
        public void Test_應用層建立進貨單_有儲存到資源庫()
        {
            var mockPurchaseOrderRepository = Substitute.For<IPurchaseOrderRepository>();
            mockPurchaseOrderRepository.Create(Arg.Any<PurchaseOrder>()).Returns(true);

            var stubPurchaseRequisitionRepository = Substitute.For<IPurchaseRequisitionRepository>();
            var stubCustomerRepository = Substitute.For<ICustomerRepository>();
            var stubWareHouseRepository = Substitute.For<IWareHouseRepository>();

            var stubValidator = Substitute.For<IStoreOrderValidator>();

            var service = new PurchaseOrderApplicationService(mockPurchaseOrderRepository, stubPurchaseRequisitionRepository, stubWareHouseRepository, stubCustomerRepository, stubValidator);

            service.CreatePurchaseOrder(new EmployeeID(1));

            mockPurchaseOrderRepository.Received().Create(Arg.Any<PurchaseOrder>());
        }

        [TestMethod]
        public void Test_應用層建立進貨單_建立失敗會回傳null()
        {
            var stubPurchaseOrderRepository = Substitute.For<IPurchaseOrderRepository>();
            stubPurchaseOrderRepository.Create(Arg.Any<PurchaseOrder>()).Returns(false);

            var stubPurchaseRequisitionRepository = Substitute.For<IPurchaseRequisitionRepository>();
            var stubCustomerRepository = Substitute.For<ICustomerRepository>();
            var stubWareHouseRepository = Substitute.For<IWareHouseRepository>();

            var stubValidator = Substitute.For<IStoreOrderValidator>();

            var service = new PurchaseOrderApplicationService(stubPurchaseOrderRepository, stubPurchaseRequisitionRepository, stubWareHouseRepository, stubCustomerRepository, stubValidator);

            var newOrder = service.CreatePurchaseOrder(new EmployeeID(1));

            Assert.AreEqual(null, newOrder);
        }
    }
}
