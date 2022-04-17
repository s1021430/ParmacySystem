using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GeneralClass.Customer;
using GeneralClass.Person;
using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.Class;
using PharmacySystem.Class.PrescriptionViewModel;
using PharmacySystem.Class.Services;
using PharmacySystem.SearchDialogs;
using PharmacySystem.View;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.Dispense
{
    public class PrescriptionViewModel : ViewModelBase
    {
        private readonly string messengerName; 
        private readonly Messenger prescriptionViewModelMessenger;
        private readonly PrescriptionApplicationService prescriptionService;
        private readonly InstitutionApplicationService institutionService;
        private readonly NHIService nhiService;
        private PrescriptionDataViewModel prescriptionData;
        public PrescriptionDataViewModel PrescriptionData
        {
            get => prescriptionData;
            private set { Set(() => PrescriptionData, ref prescriptionData, value); }
        }
        
        private MedicalOrderViewModel selectedMedicalOrder;
        public MedicalOrderViewModel SelectedMedicalOrder
        {
            get => selectedMedicalOrder;
            set { Set(() => SelectedMedicalOrder, ref selectedMedicalOrder, value); }
        }
        private int selectedMedicineIndex;
        public int SelectedMedicineIndex
        {
            get => selectedMedicineIndex;
            set { Set(() => SelectedMedicineIndex, ref selectedMedicineIndex, value); }
        }
        public ICommand GetInstitutionCommand { get; }
        public ICommand GetCustomerCommand { get; }
        public ICommand ReadCardCommand { get; }
        public ICommand SavePatientDataCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand MakeUpCommand { get; }
        public ICommand DivisionChangedCommand { get; }
        public ICommand AdjustCaseChangedCommand { get; }
        public ICommand SearchMedicineCommand { get; }
        public ICommand GetMainDiseaseCommand { get; }
        public ICommand GetSubDiseaseCommand { get; }
        public ICommand RecalculatePointCommand { get; }
        public ICommand MedicineDaysChangedCommand { get; }
        public ICommand PrescriptionRegisterCommand { get; }
        public PrescriptionViewModel(string messenger)
        {
            prescriptionViewModelMessenger = new Messenger();
            messengerName = messenger;
            SimpleIoc.Default.Register(() => prescriptionViewModelMessenger, messenger);
            nhiService = new NHIService();
            prescriptionService = PrescriptionServiceProvider.Service;
            GetInstitutionCommand = new RelayCommand<InstitutionSearchCondition>(GetInstitution);
            GetCustomerCommand = new RelayCommand<CustomerSearchCondition>(GetPatient);
            MakeUpCommand = new RelayCommand<PrescriptionRecord>(MakeUp);
            CopyCommand = new RelayCommand<PrescriptionRecord>(CopyPrescription);
            ReadCardCommand = new RelayCommand(ReadCard);
            SavePatientDataCommand = new RelayCommand(SavePatientData);
            DivisionChangedCommand = new RelayCommand(DivisionChanged);
            AdjustCaseChangedCommand = new RelayCommand(AdjustCaseChanged);
            SearchMedicineCommand = new RelayCommand<string>(SearchMedicine);
            GetMainDiseaseCommand = new RelayCommand<string>(GetMainDisease);
            GetSubDiseaseCommand = new RelayCommand<string>(GetSubDisease);
            RecalculatePointCommand = new RelayCommand(RecalculatePoint);
            MedicineDaysChangedCommand = new RelayCommand(MedicineDaysChanged);
            PrescriptionRegisterCommand = new RelayCommand(PrescriptionRegister);
            PrescriptionData = new PrescriptionDataViewModel();
        }
        ~PrescriptionViewModel()
        {
            SimpleIoc.Default.Unregister($"PrescriptionViewModelMessenger{messengerName}");
        }

        private void DivisionChanged()
        {
            PrescriptionData.CheckCopayment();
            RecalculatePoint();
        }

        private void GetInstitution(InstitutionSearchCondition condition)
        {
            InstitutionSearchPattern searchPattern;
            var searchPatternValidation = InstitutionServiceProvider.Service.SearchConditionValidate(condition, PrescriptionData.Institution);
            if (!searchPatternValidation.Success)
            {
                MessageBox.Show(searchPatternValidation.ErrorMessage);
                return;
            }
            var searchDialogViewModel = new InstitutionSearchDialogViewModel(searchPatternValidation.Result);
            var searchDialog = new InstitutionSearchDialog();
            var result = searchDialog.ProcessSearchResult(searchDialogViewModel);
            if (result == true || searchDialog.DialogResult != null && (bool) searchDialog.DialogResult)
            {
                PrescriptionData.Institution = searchDialogViewModel.SelectedInstitution;
                prescriptionViewModelMessenger.Send(new NotificationMessage("GetInstitution"));
            }
        }

        private void GetPatient(CustomerSearchCondition condition)
        {
            var searchPatternValidation = PersonService.GetSearchPattern(condition, PrescriptionData.Patient);
            if (!searchPatternValidation.Success)
            {
                MessageBox.Show(searchPatternValidation.ErrorMessage);
                return;
            }
            GetCustomer(searchPatternValidation.Result);
        }

        private void ReadCard()
        {
            PrescriptionData.Patient = nhiService.GetPatientByNHICard();
        }

        private void SavePatientData()
        {
            var customer = PrescriptionData.Patient;
            prescriptionService.SavePatientData(customer);
        }

        private void MakeUp(PrescriptionRecord data)
        {
            var prescription = prescriptionService.GetPrescriptionByID(data.ID);
            nhiService.MakeUpPrescription(prescription);
        }

        private void CopyPrescription(PrescriptionRecord record)
        {
            var copiedPrescription = prescriptionService.GetPrescriptionByID(record.ID);
            PrescriptionData = new PrescriptionDataViewModel(copiedPrescription);
        }
        
        private void AdjustCaseChanged()
        {
            RecalculatePoint();
        }

        private void SearchMedicine(string id)
        {
            if (id.Length < 4)
            {
                MessageBox.Show("查詢字數不可少於4");
                return;
            }
            var searchDialogViewModel = new MedicalOrderSearchDialogViewModel(id);
            var searchDialog = new MedicalOrderSearchDialog();
            var result = searchDialog.ProcessSearchResult(searchDialogViewModel);
            if (result == null || !(bool)result)
                return;
            var index = PrescriptionData.MedicalOrders.IndexOf(SelectedMedicalOrder);
            PrescriptionData.MedicalOrders[index] = (MedicalOrderViewModel) searchDialogViewModel.SelectedOrder;
            var needNewRow = index.Equals(PrescriptionData.MedicalOrders.Count - 1);
            if (needNewRow)
                PrescriptionData.MedicalOrders.Add(new MedicalOrderViewModel());
            SelectedMedicineIndex = index;
            prescriptionViewModelMessenger.Send(new NotificationMessage<int>(this, index, "GetMedicine"));
        }

        private void GetCustomer(CustomerSearchPattern searchPattern)
        {
            var searchDialogViewModel = new CustomerSearchDialogViewModel(searchPattern);
            var searchDialog = new CustomerSearchDialog();
            var result = searchDialog.ProcessSearchResult(searchDialogViewModel);
            if (result == true || searchDialog.DialogResult != null && (bool) searchDialog.DialogResult)
                PrescriptionData.Patient = (PatientDataViewModel) searchDialogViewModel.SelectedCustomer;
        }

        private void GetMainDisease(string id)
        {
            var disease = GetDisease(id);
            if(disease is null) return;
            PrescriptionData.MainDisease = disease;
            prescriptionViewModelMessenger.Send(new NotificationMessage("GetMainDisease"));
        }

        private void GetSubDisease(string id)
        {
            var disease = GetDisease(id);
            if (disease is null) return;
            PrescriptionData.SubDisease = disease;
            prescriptionViewModelMessenger.Send(new NotificationMessage("GetSubDisease"));
        }

        private static DiseaseCode GetDisease(string id)
        {
            if (id.Length < 4)
            {
                MessageBox.Show("查詢字數不可少於4");
                return null;
            }
            var searchDialogViewModel = new DiseaseSearchDialogViewModel(id);
            var searchDialog = new DiseaseSearchDialog();
            var result = searchDialog.ShowSearchDialog(searchDialogViewModel);
            if (result == true || searchDialog.DialogResult != null && (bool) searchDialog.DialogResult)
                return searchDialogViewModel.SelectedDiseaseCode;
            return null;
        }

        private void RecalculatePoint()
        {
            PrescriptionData.CountPoint();
            PrescriptionData.CountAmountReceivable();
        }
        
        private void MedicineDaysChanged()
        {
            PrescriptionData.CountMedicineDays();
        }

        private void PrescriptionRegister()
        {
            var result = prescriptionService.Register(prescriptionData);
        }
    }
}