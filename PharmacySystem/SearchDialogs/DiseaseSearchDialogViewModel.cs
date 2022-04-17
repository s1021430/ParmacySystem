using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem.SearchDialogs
{
    public class DiseaseSearchDialogViewModel : ViewModelBase
    {
        public DiseaseSearchDialogViewModel(string id)
        {
            service = new DiseaseCodeApplicationService();
            GetDiseases(id);
        }

        private readonly DiseaseCodeApplicationService service;
        private bool diseaseSelected;
        public bool DiseaseSelected
        {
            get => diseaseSelected;
            set
            {
                Set(() => DiseaseSelected, ref diseaseSelected, value);
            }
        }

        private List<DiseaseCode> result = new List<DiseaseCode>();
        public List<DiseaseCode> Result
        {
            get => result;
            private set
            {
                Set(() => Result, ref result, value);
            }
        }

        private DiseaseCode selectedDiseaseCode;
        public DiseaseCode SelectedDiseaseCode
        {
            get => selectedDiseaseCode;
            set
            {
                Set(() => SelectedDiseaseCode, ref selectedDiseaseCode, value);
                if (value != null)
                    DiseaseSelected = true;
            }
        }

        private void GetDiseases(string id)
        {
            Result = new List<DiseaseCode>(service.SearchDiseasesById(id));
        }
    }
}
