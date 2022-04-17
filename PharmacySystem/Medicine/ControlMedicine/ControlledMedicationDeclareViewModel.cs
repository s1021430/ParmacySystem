using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GeneralClass.Customer;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalOrders;
using GeneralClass.Product.EntityIndex;
using GeneralClass.PurchaseOrder.SearchCondition;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.PurchaseRequisition;
using PharmacySystemInfrastructure.Customer;
using PharmacySystemInfrastructure.Prescription;

namespace PharmacySystem.Medicine.ControlMedicine
{
    public class ControlledMedicationDeclareViewModel : ViewModelBase
    {
        private IPrescriptionRepository prescriptionRepository = new PrescriptionDataBaseRepository();
        private IMedicalOrderRepository medicalOrderRepository = new MedicalOrderDatabaseRepository();
        private ICustomerRepository customerRepository = new CustomerDatabaseRepository();

        private readonly List<MedicalOrderID> controlMedicalOrderCollection;

        private DateTime startDate = DateTime.Today.AddDays(-7);
        public DateTime StartDate
        {
            get => startDate;
            set => Set(ref startDate, value);
        }

        private DateTime endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => endDate;
            set => Set(ref endDate, value);
        }

        private string medcineID ;
        public string MedcineID
        {
            get => medcineID;
            set => Set(ref medcineID, value);
        }

        private List<PrescriptionData> searchPrescriptionDataList;
        public List<PrescriptionData> SearchPrescriptionDataList
        {
            get => searchPrescriptionDataList;
            set => Set(ref searchPrescriptionDataList, value);
        }
         

        private ObservableCollection<ControlMediceRecord> controlMediceRecordList = new ObservableCollection<ControlMediceRecord>();
        public ObservableCollection<ControlMediceRecord> ControlMediceRecordList
        {
            get => controlMediceRecordList;
            set => Set(ref controlMediceRecordList, value);
        }
        

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand DeclareCommand { get; set; }
        public ControlledMedicationDeclareViewModel()
        { 
          controlMedicalOrderCollection = medicalOrderRepository.GetControlMedicine();
          SearchCommand = new RelayCommand(SearchAction);
          DeclareCommand = new RelayCommand(DeclareAction);
        }

        private void DeclareAction()
        {

        }

        private void SearchAction()
        {
            ControlMediceRecordList.Clear();

            GetPrescriptionUsing();
            GetStoreOrderUsing();
        }

        private void GetStoreOrderUsing()
        {
            var purchaseOrderService = StoreOrderApplicationServiceFactory.GetPurchaseOrderApplicationService();

            foreach (var medicalOrder in controlMedicalOrderCollection)
            {
                PurchaseOrderSearchCondition condition = new PurchaseOrderSearchCondition(productID: new ProductID(medicalOrder.ID));
                IEnumerable<PurchaseOrderViewModel> searchResult = purchaseOrderService.SearchCompletePurchaseOrders(condition);
               
            } 
        }

        private void GetPrescriptionUsing()
        {
            SearchPrescriptionDataList = prescriptionRepository.GetPrescritionsByMedicalOrderIDAndDate(controlMedicalOrderCollection, StartDate, EndDate);

            foreach (var prescription in SearchPrescriptionDataList)
            {
                prescription.Patient = customerRepository.GetCustomerByCusID(prescription.PatientID);
            }

            List<MedicalOrder> medicalOrderList = new List<MedicalOrder>();

            foreach (var prescriptionMedicalOrder in SearchPrescriptionDataList.Select(_ => _.MedicalOrders))
            {
                medicalOrderList.AddRange(prescriptionMedicalOrder.Where(_ => _.ID.ID == MedcineID).ToList());
            }


            foreach (var medicalOrder in medicalOrderList)
            {
                var preMasID = medicalOrder.PreMasID;
                var pre = SearchPrescriptionDataList.Single(_ => _.ID.ID == preMasID.ID);
                ControlMediceRecord record = new ControlMediceRecord()
                {
                    HappenDate = pre.AdjustDate.ToString("yyyy-MM-dd"),
                    Reason = "調劑",
                    GetAmount = 0,
                    BatchNumber = "",
                    PayAmount = (int)medicalOrder.TotalAmount,
                    CurrentAmount = 0,
                    Remark = $"{pre.Patient.Name}({medicalOrder.TotalAmount})"
                };
                ControlMediceRecordList.Add(record);
            }
        }
    }

    public class ControlMediceRecord
    {
        public string HappenDate { get; set; } //日期

        public string Reason { get; set; } //收支原因

        public int GetAmount { get; set; } //收入數量

        public string BatchNumber { get; set; } //批號

        public int PayAmount { get; set; } //支出數量

        public int CurrentAmount { get; set; } //結存數量

        public string Remark { get; set; } //備註
    }
}
