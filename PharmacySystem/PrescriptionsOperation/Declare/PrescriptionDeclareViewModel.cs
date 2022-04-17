using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GeneralClass.Prescription.Delcare;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.Class.PrescriptionViewModel;

namespace PharmacySystem.PrescriptionsOperation.Declare
{
    public class PrescriptionDeclareViewModel: ViewModelBase
    {
        private readonly PrescriptionApplicationService service;

        private PrescriptionDeclareDataViewModel selectedDeclareData;
        public PrescriptionDeclareDataViewModel SelectedDeclareData
        {
            get => selectedDeclareData;
            set
            {
                Set(() => SelectedDeclareData, ref selectedDeclareData, value);
            }
        }

        private List<PrescriptionDeclareDataViewModel> declarePrescriptions;
        public List<PrescriptionDeclareDataViewModel> DeclarePrescriptions
        {
            get => declarePrescriptions;
            set
            {
                Set(() => DeclarePrescriptions, ref declarePrescriptions, value);
            }
        }

        private int selectedYear;
        public int SelectedYear
        {
            get => selectedYear;
            set
            {
                Set(() => SelectedYear, ref selectedYear, value);
            }
        }

        private int selectedMonth;
        public int SelectedMonth
        {
            get => selectedMonth;
            set
            {
                Set(() => SelectedMonth, ref selectedMonth, value);
            }
        }

        public List<int> DeclareYears { get; }
        public List<int> DeclareMonths => Enumerable.Range(1, 12).ToList();
        public DeclareDataSummary Summary { get; set; }
        public ICommand GetDeclarePrescriptionsCommand { get; }
        public ICommand CreateDeclareFileCommand { get; }
        public PrescriptionDeclareViewModel()
        {
            DeclareYears = new List<int> { DateTime.Now.Year, DateTime.Now.Year - 1 };
            SetSelectedDate();
            service = PrescriptionServiceProvider.Service;
            GetDeclarePrescriptionsCommand = new RelayCommand(GetDeclarePrescriptions);
            CreateDeclareFileCommand = new RelayCommand(CreateDeclareFile);
        }

        private void SetSelectedDate()
        {
            var defaultDate = DateTime.Today.AddMonths(-1);
            SelectedYear = defaultDate.Year;
            SelectedMonth = defaultDate.Month;
        }

        private void GetDeclarePrescriptions()
        {
            var (summary, prescriptions) = service.GetDeclareData(SelectedYear, SelectedMonth);
            Summary = summary;
            DeclarePrescriptions = prescriptions;
        }

        private void CreateDeclareFile()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var targetFileName = "";
            var prescriptions = DeclarePrescriptions.Where(_ => _.Declare).Select(_ => _.ID);
            service.CreateDeclareFile(prescriptions, path, targetFileName);
        }
    }
}
