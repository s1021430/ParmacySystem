using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem.SearchDialogs
{
    public class InstitutionSearchDialogViewModel : ViewModelBase
    {
        private readonly InstitutionApplicationService service;
        private bool institutionSelected;
        public bool InstitutionSelected
        {
            get => institutionSelected;
            set
            {
                Set(() => InstitutionSelected, ref institutionSelected, value);
            }
        }

        private ObservableCollection<Institution> result = new();
        public ObservableCollection<Institution> Result
        {
            get => result;
            private set
            {
                Set(() => Result, ref result, value);
            }
        }

        private Institution selectedInstitution;
        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set
            {
                Set(() => SelectedInstitution, ref selectedInstitution, value);
                if (value != null)
                    InstitutionSelected = true;
            }
        }

        public InstitutionSearchDialogViewModel(InstitutionSearchPattern searchPattern)
        {
            service = InstitutionServiceProvider.Service;
            GetInstitutions(searchPattern);
        }

        private void GetInstitutions(InstitutionSearchPattern searchPattern)
        {
            Result = searchPattern.Condition switch
            {
                InstitutionSearchCondition.ID => new ObservableCollection<Institution>(
                    service.GetInstitutionById(searchPattern.Value)),
                InstitutionSearchCondition.Name => new ObservableCollection<Institution>(
                    service.GetInstitutionByName(searchPattern.Value)),
                _ => Result
            };
            if (Result.Count > 0)
                SelectedInstitution = Result.First();
        }
    }
}
