using System.Collections.Generic;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseOrder.SearchCondition;
using GeneralClass.PurchaseRequisition.EntityIndex;

namespace GeneralClass.PurchaseOrder
{
    public interface IPurchaseOrderRepository
    {
        PurchaseOrder GetPurchaseOrderByID(PurchaseOrderID orderID);
        bool Save(PurchaseOrder order);
        bool Create(PurchaseOrder order);
        bool Delete(PurchaseOrderID orderID);
        PurchaseOrderID GetNewPurchaseOrderID();
        List<PurchaseOrder> GetDraftPurchaseOrders();
        List<PurchaseOrder> GetCompletePurchaseOrders(PurchaseOrderSearchCondition searchCondition);
    }
}
