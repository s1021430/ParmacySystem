using GeneralClass.PurchaseOrder;
using GeneralClass.PurchaseOrder.Validator;
using GeneralClass.WareHouse.EntityIndex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GeneralClassTest.StoreOrder
{
    [TestClass]
    public class StoreOrderValidatorUnitTests
    {
        private IStoreOrderValidator storeOrderValidator;

        [TestInitialize]
        public void TestPrepare()
        {
            storeOrderValidator = new PurchaseOrderValidator();
        }

        [TestMethod]
        public void Test_進貨單送出前驗證功能_送出已完成的訂單()
        {
            var stubOrder = Substitute.For<PurchaseOrder>();

            stubOrder.Status = DataStatus.Complete;

            var errorCode = storeOrderValidator.ValidateBeforeComplete(stubOrder);

            Assert.AreEqual(errorCode, PurchaseOrderErrorCode.OrderAlreadyComplete);
        }

        [TestMethod]
        public void Test_進貨單送出前驗證功能_送出錯誤的倉庫編號訂單()
        {
            var stubOrder = Substitute.For<PurchaseOrder>();

            stubOrder.WareHouseID = new WareHouseID(-1);

            var errorCode = storeOrderValidator.ValidateBeforeComplete(stubOrder);

            Assert.AreEqual(errorCode, PurchaseOrderErrorCode.WareHouseIsInvalid);
        }

        [TestMethod]
        public void Test_進貨單送出前驗證功能_送出無產品的訂單()
        {
            var stubOrder = Substitute.For<PurchaseOrder>();

            var errorCode = storeOrderValidator.ValidateBeforeComplete(stubOrder);

            Assert.AreEqual(errorCode, PurchaseOrderErrorCode.NoProduct);
        }

        [TestMethod]
        public void Test_進貨單送出前驗證功能_送出產品數量為零的訂單()
        {
            var stubOrder = Substitute.For<PurchaseOrder>();
            var stubProduct = Substitute.For<OrderProduct>();

            stubProduct.Amount = 0;
            stubProduct.FreeAmount = 0;

            stubOrder.Products.Add(stubProduct);

            var errorCode = storeOrderValidator.ValidateBeforeComplete(stubOrder);

            Assert.AreEqual(errorCode, PurchaseOrderErrorCode.ProductOrderDontHaveAmount);
        }

        [TestMethod]
        public void Test_進貨單送出前驗證功能_送出產品價格為負的訂單()
        {
            var stubOrder = Substitute.For<PurchaseOrder>();
            var stubProduct = Substitute.For<OrderProduct>();

            stubProduct.Amount = 1;
            stubProduct.Price = -1;

            stubOrder.Products.Add(stubProduct);

            var errorCode = storeOrderValidator.ValidateBeforeComplete(stubOrder);

            Assert.AreEqual(errorCode, PurchaseOrderErrorCode.ProductPriceIsNegative);
        }
    }
}
