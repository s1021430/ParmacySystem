using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GeneralClass.Employee;
using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.SearchDialogs;
using PharmacySystem.View;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.Search
{
    public class PrescriptionSearchViewModel : ViewModelBase
    {
        private readonly PrescriptionApplicationService prescriptionService;
        public List<AdjustCase> AdjustCaseList { get; }
        public List<Employee> PharmacistList { get; }
        private Institution institution = new();
        public Institution Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
            }
        }

        private DateTime? patientBirthday;
        public DateTime? PatientBirthday
        {
            get => patientBirthday;
            set
            {
                Set(() => PatientBirthday, ref patientBirthday, value);
            }
        }
        private string patientIdNumber;
        public string PatientIdNumber
        {
            get => patientIdNumber;
            set
            {
                Set(() => PatientIdNumber, ref patientIdNumber, value);
            }
        }
        private string patientName;
        public string PatientName
        {
            get => patientName;
            set
            {
                Set(() => PatientName, ref patientName, value);
            }
        }
        private PrescriptionRecord? selectedRecord;
        public PrescriptionRecord? SelectedRecord
        {
            get => selectedRecord;
            set
            {
                Set(() => SelectedRecord, ref selectedRecord, value);
            }
        }
        private List<PrescriptionRecord> prescriptionsList = new();
        public List<PrescriptionRecord> PrescriptionsList
        {
            get => prescriptionsList;
            private set
            {
                Set(() => PrescriptionsList, ref prescriptionsList, value);
            }
        }
        private AdjustCase adjustCase;
        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                Set(() => AdjustCase, ref adjustCase, value);
            }
        }

        private Employee pharmacist;
        public Employee Pharmacist
        {
            get => pharmacist;
            set
            {
                Set(() => Pharmacist, ref pharmacist, value);
            }
        }

        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        public PrescriptionSearchViewModel()
        {
            AdjustCaseList = PrescriptionApplicationService.GetAdjustCaseList();
            prescriptionService = PrescriptionServiceProvider.Service;
            PharmacistList = EmployeeServiceProvider.Service.GetAllPharmacist();
            GetInstitutionCommand = new RelayCommand<InstitutionSearchCondition>(GetInstitution);
            SearchCommand = new RelayCommand(SearchPrescriptions);
            OpenPrescriptionCommand = new RelayCommand(OpenPrescription);
        }

        public ICommand GetInstitutionCommand { get; }
        public ICommand SearchCommand { get; set; }
        public ICommand OpenPrescriptionCommand { get; set; }
        private void GetInstitution(InstitutionSearchCondition condition)
        {
            InstitutionSearchPattern searchPattern;
            switch (condition)
            {
                case InstitutionSearchCondition.ID:
                    searchPattern = new InstitutionSearchPattern(condition, Institution.ID);
                    break;
                case InstitutionSearchCondition.Name:
                    searchPattern = new InstitutionSearchPattern(condition, Institution.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition), condition, null);
            }
            var searchDialogViewModel = new InstitutionSearchDialogViewModel(searchPattern);
            var searchDialog = new InstitutionSearchDialog();
            var result = searchDialog.ProcessSearchResult(searchDialogViewModel);
            if (result == true || searchDialog.DialogResult != null && (bool)searchDialog.DialogResult)
                Institution = searchDialogViewModel.SelectedInstitution;
        }

        private void SearchPrescriptions()
        {
            var searchPattern = new PrescriptionSearchPattern()
            {
                StartDate = startDate,
                EndDate = endDate.AddDays(1),
                InstitutionId = institution?.ID,
                PatientIdNumber = patientIdNumber,
                PatientBirthday = patientBirthday,
                PatientName = patientName,
                AdjustCaseId = adjustCase.AdjustCaseID.ID,
                PharmacistId = pharmacist?.ID.ID
            };
            PrescriptionsList = prescriptionService.SearchPrescription(searchPattern);
            RaisePropertyChanged(nameof(PrescriptionsList));
        }

        private void OpenPrescription()
        {
            if (SelectedRecord is null)
            {
                MessageBox.Show("尚未選擇處方");
                return;
            }

            var prescription = prescriptionService.GetPrescriptionByID(SelectedRecord.ID);
            ViewModelLocator.Locator.Main.OpenPrescriptionEditCommand.Execute(prescription);
        }
    }
}
