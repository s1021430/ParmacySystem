using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.PurchaseOrder.EntityIndex;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.PurchaseRequisition;

namespace PharmacySystem.OrdersOperation
{
    public class StoreOrderManagementViewModel : ViewModelBase
    {
        private EmployeeID tempID = new EmployeeID(0);

        #region ----- Commands -----
        public RelayCommand ReloadCommand { get; }
        public RelayCommand AddPurchaseOrderCommand { get; }
        public RelayCommand DeletePurchaseOrderCommand { get; }
        public RelayCommand BringInPurchaseRequisitionCommand { get; }
        #endregion

        #region ----- Variables -----
        private readonly PurchaseOrderApplicationService applicationService;

        private ObservableCollection<PurchaseOrderViewModel> purchaseOrderViewModels;
        public ObservableCollection<PurchaseOrderViewModel> PurchaseOrderViewModels
        {
            get => purchaseOrderViewModels;
            set => Set(ref purchaseOrderViewModels, value);
        }

        private PurchaseOrderViewModel selectedPurchaseOrderViewModel;
        public PurchaseOrderViewModel SelectedPurchaseOrderViewModel
        {
            get => selectedPurchaseOrderViewModel;
            set
            {
                var result = SaveCurrentPurchaseOrder();

                if(!result) return;

                Set(ref selectedPurchaseOrderViewModel, value);
            }
        }
        #endregion

        public StoreOrderManagementViewModel()
        {
            applicationService = StoreOrderApplicationServiceFactory.GetPurchaseOrderApplicationService();

            ReloadCommand = new RelayCommand(ReloadAction);
            AddPurchaseOrderCommand = new RelayCommand(AddPurchaseOrderAction);
            DeletePurchaseOrderCommand = new RelayCommand(DeletePurchaseOrderAction);
            BringInPurchaseRequisitionCommand = new RelayCommand(BringInPurchaseRequisitionAction);

            ReloadAction();
        }

        #region ----- Actions -----
        private void ReloadAction()
        {
            PurchaseOrderViewModels = applicationService.GetDraftPurchaseOrders();

            if (PurchaseOrderViewModels.Any())
                SelectedPurchaseOrderViewModel = PurchaseOrderViewModels.First();
        }

        private void AddPurchaseOrderAction()
        {
            var newPurchaseOrderViewModel = applicationService.CreatePurchaseOrder(tempID);

            if (newPurchaseOrderViewModel == null)
            {
                //Error Message
            }
            else
            {
                PurchaseOrderViewModels.Add(newPurchaseOrderViewModel);
                SelectedPurchaseOrderViewModel = newPurchaseOrderViewModel;
            }
        }

        private void DeletePurchaseOrderAction()
        {
            if (selectedPurchaseOrderViewModel == null) return;
            
            //Ask if user need delete

            var result = applicationService.DeletePurchaseOrder(new PurchaseOrderID(selectedPurchaseOrderViewModel.ID), tempID);

            if (result)
            {
                purchaseOrderViewModels.Remove(selectedPurchaseOrderViewModel);

                if (purchaseOrderViewModels.Any())
                    selectedPurchaseOrderViewModel = purchaseOrderViewModels.First();
            }
            else
            {
                //Error Message
            }
        }

        private void BringInPurchaseRequisitionAction()
        {
            var window = new PurchaseRequisitionSearchDialog();
            var result = window.ShowDialog();

            if (result is null or false) return;
            
            var selectedID = ((PurchaseRequisitionSearchDialogViewModel) window.DataContext).SelectedPurchaseRequisitionViewModel.ID;
            var bringInResult = applicationService.BringInPurchaseRequisitionData(selectedID, new PurchaseOrderID(selectedPurchaseOrderViewModel.ID));

            if (bringInResult == null)
            {
                //Error Message
            }
            else
            {
                var originalIndex = PurchaseOrderViewModels.IndexOf(selectedPurchaseOrderViewModel);
                PurchaseOrderViewModels[originalIndex] = bringInResult;

                SelectedPurchaseOrderViewModel = bringInResult;
            }
        }
        #endregion

        #region ----- Functions -----
        private bool SaveCurrentPurchaseOrder()
        {
            return applicationService.EditPurchaseOrder(selectedPurchaseOrderViewModel);
        }
        #endregion
    }
}
