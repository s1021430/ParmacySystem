using GeneralClass.Employee.EntityIndex;
using GeneralClass.PurchaseRequisition.EntityIndex;

namespace GeneralClass.PurchaseRequisition
{
    public class PurchaseRequisitionFactory
    {
        public PurchaseRequisition CreatePurchaseRequisition(PurchaseRequisitionID newPurchaseRequisitionID, EmployeeID creatorID)
        {
            return new(newPurchaseRequisitionID, creatorID);
        }
    }
}
