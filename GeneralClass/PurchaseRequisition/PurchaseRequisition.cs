using System.Collections.Generic;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.Manufactory.EntityIndex;
using GeneralClass.Product.EntityIndex;
using GeneralClass.PurchaseRequisition.EntityIndex;
using GeneralClass.WareHouse.EntityIndex;

namespace GeneralClass.PurchaseRequisition
{

    public class PurchaseRequisition
    {
        private EmployeeID creatorID;

        public PurchaseRequisitionID ID { get; }
        public ManufactoryID ManufactoryID { get; }
        public WareHouseID WareHouseID { get; }
        public string Note { get; }
        public List<RequisitionProduct> Products { get; } = new();

        public PurchaseRequisition(PurchaseRequisitionID newPurchaseRequisitionID, EmployeeID creatorID)
        {
            ID = newPurchaseRequisitionID;
            this.creatorID = creatorID;
        }

        public PurchaseRequisition()
        {

        }
    }

    public class RequisitionProduct
    {
        public ProductID ID { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public string Note { get; set; }

        public RequisitionProduct()
        {

        }
    }
}
