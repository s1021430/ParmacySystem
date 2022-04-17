using GalaSoft.MvvmLight;
using GeneralClass.PurchaseRequisition.EntityIndex;

namespace PharmacySystem.PurchaseRequisition
{
    public class PurchaseRequisitionViewModel : ObservableObject
    {
        public static explicit operator PurchaseRequisitionViewModel(GeneralClass.PurchaseRequisition.PurchaseRequisition purchaseRequisition) => new(purchaseRequisition);

        #region ----- Variables -----
        private PurchaseRequisitionID id;
        public PurchaseRequisitionID ID
        {
            get => id;
            set => Set(ref id, value);
        }
        #endregion
        
        private PurchaseRequisitionViewModel(GeneralClass.PurchaseRequisition.PurchaseRequisition purchaseRequisition)
        {
            id = purchaseRequisition.ID;

        }
    }
}
