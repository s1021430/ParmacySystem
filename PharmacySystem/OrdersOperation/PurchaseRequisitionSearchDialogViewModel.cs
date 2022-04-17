using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem.PurchaseRequisition
{
    public delegate void SearchDone();

    public class PurchaseRequisitionSearchDialogViewModel : ViewModelBase
    {
        #region ----- Commands -----
        public RelayCommand SearchCommand { get; }
        public RelayCommand SelectPurchaseRequisitionCommand { get; }
        #endregion

        #region ----- Variables -----
        private SearchDone searchDone;
        private readonly PurchaseRequisitionApplicationService applicationService;
        
        private ObservableCollection<PurchaseRequisitionViewModel> purchaseRequisitionViewModels;
        public ObservableCollection<PurchaseRequisitionViewModel> PurchaseRequisitionViewModels
        {
            get => purchaseRequisitionViewModels;
            set => Set(ref purchaseRequisitionViewModels, value);
        }

        private PurchaseRequisitionViewModel selectedPurchaseRequisitionViewModel;
        public PurchaseRequisitionViewModel SelectedPurchaseRequisitionViewModel
        {
            get => selectedPurchaseRequisitionViewModel;
            set => Set(ref selectedPurchaseRequisitionViewModel, value);
        }
        #endregion

        public PurchaseRequisitionSearchDialogViewModel(SearchDone searchDoneDelegate)
        {
            applicationService = PurchaseRequisitionApplicationServiceFactory.GetPurchaseRequisitionApplicationService();

            searchDone = searchDoneDelegate;

            SearchCommand = new RelayCommand(SearchAction);
            SelectPurchaseRequisitionCommand = new RelayCommand(SelectPurchaseRequisitionAction);

            SearchAction();
        }

        #region ----- Actions -----
        private void SearchAction()
        {
            PurchaseRequisitionViewModels = applicationService.GetPurchaseRequisitions();

            if (purchaseRequisitionViewModels.Any())
                SelectedPurchaseRequisitionViewModel = purchaseRequisitionViewModels.First();
        }

        private void SelectPurchaseRequisitionAction()
        {
            if (selectedPurchaseRequisitionViewModel == null)
            {
                //Show Message
            }
            
            searchDone();
        }
        #endregion
    }
}
