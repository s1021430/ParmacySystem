using System.Collections.Generic;
using GeneralClass.PurchaseRequisition.EntityIndex;

namespace GeneralClass.PurchaseRequisition
{
    public interface IPurchaseRequisitionRepository
    {
        PurchaseRequisition GetPurchaseRequisitionByID(PurchaseRequisitionID requisitionID);
        bool Save(PurchaseRequisition requisition);
        bool Create(PurchaseRequisition requisition);
        PurchaseRequisitionID GetNewPurchaseRequisitionID();
        List<PurchaseRequisition> GetPurchaseRequisitions();
    }
}
