using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystemInfrastructure.Prescription;

namespace PharmacySystem.ApplicationServiceLayer
{
    public class DiseaseCodeApplicationService
    {
        private readonly IDiseaseCodeRepository diseaseCodeRepository = new DiseaseCodeDatabaseRepository();

        public List<DiseaseCode> SearchDiseasesById(string id)
        {
            return diseaseCodeRepository.SearchDiseasesById(id);
        }

        public DiseaseCode GetDiseasesById(string id)
        {
            return diseaseCodeRepository.GetDiseasesById(id);
        }

        public bool IsExist(string id)
        {
            return SearchDiseasesById(id).Any();
        }
    }
}
