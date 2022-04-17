using System.Collections.ObjectModel;
using System.Linq;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.PurchaseRequisition;
using PharmacySystem.PurchaseRequisition;
using PharmacySystemInfrastructure.StoreOrder;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class PurchaseRequisitionApplicationServiceFactory
    {
        public static PurchaseRequisitionApplicationService GetPurchaseRequisitionApplicationService()
        {
            var purchaseRequisitionRepository = new PurchaseRequisitionDatabaseRepository();

            return new PurchaseRequisitionApplicationService(purchaseRequisitionRepository);
        }
    }

    public class PurchaseRequisitionApplicationService
    {
        private readonly IPurchaseRequisitionRepository purchaseRequisitionRepository;
        private readonly PurchaseRequisitionFactory purchaseRequisitionFactory;

        public PurchaseRequisitionApplicationService(IPurchaseRequisitionRepository purchaseRequisitionRepository)
        {
            this.purchaseRequisitionRepository = purchaseRequisitionRepository;

            purchaseRequisitionFactory = new PurchaseRequisitionFactory();
        }

        public ObservableCollection<PurchaseRequisitionViewModel> GetPurchaseRequisitions()
        {
            var purchaseRequisitions = purchaseRequisitionRepository.GetPurchaseRequisitions();
            var purchaseRequisitionViewModels = purchaseRequisitions.Select(_ => (PurchaseRequisitionViewModel)_);

            return new ObservableCollection<PurchaseRequisitionViewModel>(purchaseRequisitionViewModels);
        }

        public GeneralClass.PurchaseRequisition.PurchaseRequisition CreatePurchaseRequisition(EmployeeID creatorID)
        {
            var newPurchaseRequisitionID = purchaseRequisitionRepository.GetNewPurchaseRequisitionID();
            var newRequisition = purchaseRequisitionFactory.CreatePurchaseRequisition(newPurchaseRequisitionID, creatorID);

            var isSuccess = purchaseRequisitionRepository.Save(newRequisition);

            return isSuccess ? newRequisition : null;
        }

    }
}
