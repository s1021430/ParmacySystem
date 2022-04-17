using GeneralClass.Employee.EntityIndex;
using GeneralClass.PurchaseOrder.EntityIndex;

namespace GeneralClass.PurchaseOrder
{
    public class PurchaseOrderFactory
    {
        public PurchaseOrder CreatePurchaseOrder(PurchaseOrderID newPurchaseOrderID, EmployeeID creatorID)
        {
            return new(newPurchaseOrderID, creatorID);
        }
    }
}
