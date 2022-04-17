using System.Linq;
using GeneralClass.Customer.EntityIndex;
using GeneralClass.Manufactory.EntityIndex;
using GeneralClass.Product.EntityIndex;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseRequisition.EntityIndex;
using GeneralClass.WareHouse.EntityIndex;

namespace GeneralClass.PurchaseOrder.SearchCondition
{
    public readonly struct PurchaseOrderSearchCondition
    {
        public PurchaseOrderID? ID { get; }
        public PurchaseRequisitionID? RequisitionID { get; }
        public ManufactoryID? ManufactoryID { get; }
        public WareHouseID? WareHouseID { get; }
        public CustomerID? CustomerID { get; }
        public ProductID? ProductID { get; }

        public PurchaseOrderSearchCondition(PurchaseOrderID? id = null, PurchaseRequisitionID? requisitionID = null,
            ManufactoryID? manufactoryID = null, WareHouseID? wareHouseID = null, CustomerID? customerID = null, ProductID? productID = null)
        {
            ID = id;
            RequisitionID = requisitionID;
            ManufactoryID = manufactoryID;
            WareHouseID = wareHouseID;
            CustomerID = customerID;
            ProductID = productID;
        }

        public bool HasCondition()
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetValue(this) != null) return true;
            }

            return false;
        }
    }
}
