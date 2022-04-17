using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dragablz;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PharmacySystem.ViewModel;

namespace PharmacySystem.Medicine.Management
{
    public class MedicineManagementViewModel: ViewModelBase
    {
        public bool CanAddMedicineDetail => MedicinesTabs.Count < 15;
        public RelayCommand OpenMedicineDetailCommand { get; }
        public ObservableCollection<HeaderedItemViewModel> MedicinesTabs { get; }
        private HeaderedItemViewModel selectedMedicineDetail;
        public HeaderedItemViewModel SelectedMedicineDetail
        {
            get => selectedMedicineDetail;
            set
            {
                if (selectedMedicineDetail == value)
                    return;
                Set(() => SelectedMedicineDetail, ref selectedMedicineDetail, value);
            }
        }

        public MedicineManagementViewModel()
        {
            MedicinesTabs = new ObservableCollection<HeaderedItemViewModel>
            {
                new HeaderedItemViewModel(new TabItemViewModel("",true),
                    new MedicineDetailView())
            };
            OpenMedicineDetailCommand = new RelayCommand(OpenMedicineDetail);
            MedicinesTabs.CollectionChanged += MedicinesTabsOnCollectionChanged;
        }

        ~MedicineManagementViewModel()
        {
            MedicinesTabs.CollectionChanged -= MedicinesTabsOnCollectionChanged;
        }

        private void MedicinesTabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(MedicinesTabs));
            SelectedMedicineDetail = MedicinesTabs.Last();
            RaisePropertyChanged(nameof(CanAddMedicineDetail));
        }

        private void OpenMedicineDetail()
        {
            var medicineDetailTab = new HeaderedItemViewModel(new TabItemViewModel(""), null);
            MedicinesTabs.Add(medicineDetailTab);
            medicineDetailTab.Content = new MedicineDetailView();
        }
    }
}
