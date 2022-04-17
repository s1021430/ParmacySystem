using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dragablz;
using GalaSoft.MvvmLight;
using GeneralClass.Prescription;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.DispenseEdit
{
    public class DispenseEditViewModel : ViewModelBase
    {
        public bool CanAddPrescription => PrescriptionsTabs.Count < 15;
        public ObservableCollection<HeaderedItemViewModel> PrescriptionsTabs { get; }

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

        public DispenseEditViewModel()
        {
            PrescriptionsTabs = new ObservableCollection<HeaderedItemViewModel>();
            PrescriptionsTabs.CollectionChanged += PrescriptionsTabsOnCollectionChanged;
        }

        ~DispenseEditViewModel()
        {
            PrescriptionsTabs.CollectionChanged -= PrescriptionsTabsOnCollectionChanged;
        }

        private void PrescriptionsTabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(PrescriptionsTabs));
            SelectedTab = PrescriptionsTabs.Any() ? PrescriptionsTabs.Last() : null;
            RaisePropertyChanged(nameof(CanAddPrescription));
        }

        public void AddPrescription(PrescriptionData prescription)
        {
            var existPrescription =
                PrescriptionsTabs.FirstOrDefault(p => ((PrescriptionEditView) p.Content).ID.Equals(prescription.ID));
            if (existPrescription != null)
            {
                SelectedTab = existPrescription;
                return;
            }
            var newPrescriptionTab = new HeaderedItemViewModel(new TabItemViewModel(prescription.Patient?.Name), null);
            PrescriptionsTabs.Add(newPrescriptionTab);
            newPrescriptionTab.Content = new PrescriptionEditView(prescription);
        }
    }
}
