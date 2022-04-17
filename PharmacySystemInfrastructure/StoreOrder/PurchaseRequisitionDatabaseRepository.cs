using System;
using System.Collections.Generic;
using GeneralClass.PurchaseRequisition;
using GeneralClass.PurchaseRequisition.EntityIndex;

namespace PharmacySystemInfrastructure.StoreOrder
{
    public class PurchaseRequisitionDatabaseRepository : IPurchaseRequisitionRepository
    {
        public PurchaseRequisition GetPurchaseRequisitionByID(PurchaseRequisitionID requisitionID)
        {
            throw new NotImplementedException();
        }

        public bool Save(PurchaseRequisition requisition)
        {
            throw new NotImplementedException();
        }

        public bool Create(PurchaseRequisition requisition)
        {
            throw new NotImplementedException();
        }

        public PurchaseRequisitionID GetNewPurchaseRequisitionID()
        {
            var randomID = GenerateRandomID();
            return new PurchaseRequisitionID($"PR-{DateTime.Today:yyyyMMdd}-{randomID}");
        }

        private string GenerateRandomID()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
        }

        public List<PurchaseRequisition> GetPurchaseRequisitions()
        {
            throw new NotImplementedException();
        }

    }
}
