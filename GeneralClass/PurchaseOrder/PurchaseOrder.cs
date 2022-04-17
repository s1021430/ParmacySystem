using System;
using System.Collections.Generic;
using GeneralClass.Customer.EntityIndex;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.Manufactory.EntityIndex;
using GeneralClass.Product.EntityIndex;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseRequisition;
using GeneralClass.PurchaseRequisition.EntityIndex;
using GeneralClass.WareHouse.EntityIndex;

namespace GeneralClass.PurchaseOrder
{
    public enum DataStatus
    {
        Draft = 1,
        Complete = 2
    }
    
    public class PurchaseOrder
    {
        private EmployeeID creatorID;
        private EmployeeID completeID;

        public PurchaseOrderID ID { get; }
        public PurchaseRequisitionID RequisitionID { get; private set; }
        public ManufactoryID ManufactoryID { get; set; }
        public WareHouseID WareHouseID { get; set; }
        public DataStatus Status { get; set; }
        public DateTime CompleteTime { get; set; }
        public CustomerID CustomerID { get; set; }
        public string Note { get; set; }
        public List<OrderProduct> Products { get; } = new();

        public PurchaseOrder() {}

        public PurchaseOrder(PurchaseOrderID id, EmployeeID creatorID)
        {
            ID = id;
            this.creatorID = creatorID;
        }

        public void BringIn(PurchaseRequisition.PurchaseRequisition purchaseRequisition)
        {
            if (Status == DataStatus.Complete)
                throw new Exception("Cant Edit Complete Order.");

            RequisitionID = purchaseRequisition.ID;
            ManufactoryID = purchaseRequisition.ManufactoryID;
            WareHouseID = purchaseRequisition.WareHouseID;
            Note = purchaseRequisition.Note;

            Products.Clear();

            foreach (var requisitionProduct in purchaseRequisition.Products)
                Products.Add(new OrderProduct(requisitionProduct));
        }

        public void Complete(EmployeeID completeEmployeeID)
        {
            Status = DataStatus.Complete;
            completeID = completeEmployeeID;
            CompleteTime = DateTime.Now;
        }
    }

    public class OrderProduct
    {
        public OrderProduct()
        {

        }

        public OrderProduct(RequisitionProduct requisitionProduct)
        {
            ID = requisitionProduct.ID;
            Amount = requisitionProduct.Amount;
            SubTotal = requisitionProduct.SubTotal;
            Note = requisitionProduct.Note;
        }
        
        public ProductID ID { get; set; }
        public int Amount { get; set; }
        public int FreeAmount { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime ValidDate { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }
    }
}
