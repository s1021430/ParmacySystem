using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GeneralClass.Prescription.MedicalOrders;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem.SearchDialogs
{
    public class MedicalOrderSearchDialogViewModel : ViewModelBase
    {
        private readonly MedicalOrderApplicationService service;
        private bool orderSelected;
        public bool OrderSelected
        {
            get => orderSelected;
            set
            {
                Set(() => OrderSelected, ref orderSelected, value);
            }
        }

        private List<MedicalOrder> result = new();
        public List<MedicalOrder> Result
        {
            get => result;
            private set
            {
                Set(() => Result, ref result, value);
            }
        }

        private MedicalOrder selectedOrder;
        public MedicalOrder SelectedOrder
        {
            get => selectedOrder;
            set
            {
                Set(() => SelectedOrder, ref selectedOrder, value);
                if (value != null)
                    OrderSelected = true;
            }
        }

        public MedicalOrderSearchDialogViewModel(string id)
        {
            service = MedicalOrderApplicationServiceProvider.Service;
            GetMedicalOrders(id);
        }

        private void GetMedicalOrders(string id)
        {
            Result = new List<MedicalOrder>(service.SearchMedicalOrdersByID(id));
        }
    }
}
