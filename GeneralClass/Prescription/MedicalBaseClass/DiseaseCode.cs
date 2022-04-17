using System.Collections.Generic;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    public class DiseaseCode
    {
        public  DiseaseCode(){}
        public DiseaseCode(DiseaseCodeID id, string name)
        {
            ID = id;
            ChineseName = name;
        }
        public DiseaseCodeID ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public int Type { get; set; }
    }

    public interface IDiseaseCodeRepository
    {
        DiseaseCode GetDiseasesById(string id);
        List<DiseaseCode> SearchDiseasesById(string id);
    }
}
