using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dragablz;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GeneralClass.Employee;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.Dispense
{
    public class DispenseViewModel : ViewModelBase
    {
        public bool CanAddPrescription => PrescriptionsTabs.Count < 15;
        public RelayCommand NewPrescriptionCommand { get; }
        public ObservableCollection<HeaderedItemViewModel> PrescriptionsTabs { get; }
        public List<Copayment> CopaymentList { get; } = PrescriptionApplicationService.GetCopaymentList();
        public List<AdjustCase> AdjustCaseList { get; } = PrescriptionApplicationService.GetAdjustCaseList();
        public List<Division> DivisionList { get; } = PrescriptionApplicationService.GetDivisionList();
        public List<Employee> PharmacistList { get; }
        public List<PaymentCategory> PaymentCategoryList { get; } = PrescriptionApplicationService.GetPaymentCategoryList();
        public List<PrescriptionCase> PrescriptionCaseList { get; } = PrescriptionApplicationService.GetPrescriptionCaseList();
        public List<SpecialTreat> SpecialTreatList { get; } = PrescriptionApplicationService.GetSpecialTreatList();

        private HeaderedItemViewModel selectedTab;
        public HeaderedItemViewModel SelectedTab
        {
            get => selectedTab;
            set
            {
                if (selectedTab == value)
                    return;
                Set(() => SelectedTab, ref selectedTab, value);
            }
        }

        public DispenseViewModel()
        {
            PharmacistList = EmployeeServiceProvider.Service.GetAllPharmacist();
            PrescriptionsTabs = new ObservableCollection<HeaderedItemViewModel>();
            NewPrescriptionCommand = new RelayCommand(NewPrescription);
            PrescriptionsTabs.CollectionChanged += PrescriptionsTabsOnCollectionChanged;
        }

        ~ DispenseViewModel()
        {
            PrescriptionsTabs.CollectionChanged -= PrescriptionsTabsOnCollectionChanged;
        }

        private void PrescriptionsTabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(PrescriptionsTabs));
            SelectedTab = PrescriptionsTabs.Last();
            RaisePropertyChanged(nameof(CanAddPrescription));
            if (PrescriptionsTabs.Count == 1)
                ((TabItemViewModel)PrescriptionsTabs[0].Header).Closable = false;
            else
            {
                foreach (var p in PrescriptionsTabs)
                    ((TabItemViewModel)p.Header).Closable = true;
            }
        }

        private void NewPrescription()
        {
            var newPrescriptionTab = new HeaderedItemViewModel(new TabItemViewModel("新處方"), null);
            PrescriptionsTabs.Add(newPrescriptionTab);
            newPrescriptionTab.Content = new PrescriptionView();
        }
    }
}
