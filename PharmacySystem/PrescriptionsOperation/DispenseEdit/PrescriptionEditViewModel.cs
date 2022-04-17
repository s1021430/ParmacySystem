using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GeneralClass;
using GeneralClass.Customer;
using GeneralClass.Person;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Prescription.Validation;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.Class;
using PharmacySystem.Class.PrescriptionViewModel;
using PharmacySystem.Class.Services;
using PharmacySystem.SearchDialogs;
using PharmacySystem.View;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.DispenseEdit
{
    public class PrescriptionEditViewModel : ViewModelBase
    {
        private readonly string messengerName;
        private readonly Messenger prescriptionEditViewModelMessenger;
        private readonly PrescriptionApplicationService service = PrescriptionServiceProvider.Service;
        private readonly NHIService nhiService = NHIServiceProvider.Service;

        #region Properties

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

        #endregion

        #region Commands
        public ICommand MakeUpCommand { get; }
        public ICommand SavePatientDataCommand { get; }
        public ICommand GetCustomerCommand { get; }
        public ICommand GetInstitutionCommand { get; }
        public ICommand DivisionChangedCommand { get; }
        public ICommand AdjustCaseChangedCommand { get; }
        public ICommand SearchMedicineCommand { get; }
        public ICommand GetMainDiseaseCommand { get; }
        public ICommand GetSubDiseaseCommand { get; }
        public ICommand RecalculatePointCommand { get; }
        public ICommand MedicineDaysChangedCommand { get; }
        public ICommand PrescriptionDeleteCommand { get; }
        public ICommand PrescriptionSaveCommand { get; }

        #endregion

        public PrescriptionEditViewModel(PrescriptionData prescription)
        {
            prescriptionEditViewModelMessenger = new Messenger();
            messengerName = $"PrescriptionEditViewModelMessenger{prescription.ID}";
            SimpleIoc.Default.Register(() => prescriptionEditViewModelMessenger, messengerName);
            GetCustomerCommand = new RelayCommand<CustomerSearchCondition>(GetPatient);
            MakeUpCommand = new RelayCommand(MakeUp);
            SavePatientDataCommand = new RelayCommand(SavePatientData);
            GetInstitutionCommand = new RelayCommand<InstitutionSearchCondition>(GetInstitution);
            DivisionChangedCommand = new RelayCommand(DivisionChanged);
            AdjustCaseChangedCommand = new RelayCommand(AdjustCaseChanged);
            SearchMedicineCommand = new RelayCommand<string>(SearchMedicine);
            GetMainDiseaseCommand = new RelayCommand<string>(GetMainDisease);
            GetSubDiseaseCommand = new RelayCommand<string>(GetSubDisease);
            RecalculatePointCommand = new RelayCommand(RecalculatePoint);
            MedicineDaysChangedCommand = new RelayCommand(MedicineDaysChanged);
            PrescriptionDeleteCommand = new RelayCommand(DeletePrescription);
            PrescriptionSaveCommand = new RelayCommand(SavePrescription);
            service.ClearMedicalServiceOrder(prescription);
            PrescriptionData = (PrescriptionDataViewModel)prescription;
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
            if (result == true || searchDialog.DialogResult != null && (bool)searchDialog.DialogResult)
            {
                PrescriptionData.Institution = searchDialogViewModel.SelectedInstitution;
                prescriptionEditViewModelMessenger.Send(new NotificationMessage("GetInstitution"));
            }
        }

        private void DivisionChanged()
        {
            PrescriptionData.CheckCopayment();
            RecalculatePoint();
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
            PrescriptionData.MedicalOrders[index] = (MedicalOrderViewModel)searchDialogViewModel.SelectedOrder;
            var needNewRow = index.Equals(PrescriptionData.MedicalOrders.Count - 1);
            if (needNewRow)
                PrescriptionData.MedicalOrders.Add(new MedicalOrderViewModel());
            SelectedMedicineIndex = index;
            prescriptionEditViewModelMessenger.Send(new NotificationMessage<int>(this, index, "GetMedicine"));
        }

        private void GetPatient(CustomerSearchCondition condition)
        {
            var searchPatternValidation = PersonService.GetSearchPattern(condition,PrescriptionData.Patient);
            if (!searchPatternValidation.Success)
            {
                MessageBox.Show(searchPatternValidation.ErrorMessage);
                return;
            }
            GetCustomer(searchPatternValidation.Result);
        }

        private void GetCustomer(CustomerSearchPattern searchPattern)
        {
            var searchDialogViewModel = new CustomerSearchDialogViewModel(searchPattern);
            var searchDialog = new CustomerSearchDialog();
            var result = searchDialog.ProcessSearchResult(searchDialogViewModel);
            if (result == true || searchDialog.DialogResult != null && (bool)searchDialog.DialogResult)
                PrescriptionData.Patient = (PatientDataViewModel)searchDialogViewModel.SelectedCustomer;
        }

        private void MakeUp()
        {
            PrescriptionData.Patient = nhiService.GetPatientByNHICard();
        }

        private void SavePatientData()
        {
            var customer = PrescriptionData.Patient;
            service.SavePatientData(customer);
        }

        private void GetMainDisease(string id)
        {
            var disease = GetDisease(id);
            if (disease is null) return;
            PrescriptionData.MainDisease = GetDisease(id);
            prescriptionEditViewModelMessenger.Send(new NotificationMessage("GetMainDisease"));
        }

        private void GetSubDisease(string id)
        {
            var disease = GetDisease(id);
            if (disease is null) return;
            PrescriptionData.SubDisease = GetDisease(id);
            prescriptionEditViewModelMessenger.Send(new NotificationMessage("GetSubDisease"));
        }

        private static DiseaseCode GetDisease(string id)
        {
            if (id.Length < 4)
            {
                MessageBox.Show("查詢字數不可少於4");
                return new DiseaseCode(new DiseaseCodeID(string.Empty), "");
            }
            var searchDialogViewModel = new DiseaseSearchDialogViewModel(id);
            var searchDialog = new DiseaseSearchDialog();
            var result = searchDialog.ShowSearchDialog(searchDialogViewModel);
            if (result == true || searchDialog.DialogResult != null && (bool)searchDialog.DialogResult)
                return searchDialogViewModel.SelectedDiseaseCode;
            return new DiseaseCode(new DiseaseCodeID(string.Empty), "");
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
        private void DeletePrescription()
        {
            service.Delete(PrescriptionData);
        }

        private void SavePrescription()
        {
            var result = service.Save(prescriptionData);
            if (result != PrescriptionErrorCode.Success)
                MessageBox.Show(result.GetDescription());
        }
    }
}
