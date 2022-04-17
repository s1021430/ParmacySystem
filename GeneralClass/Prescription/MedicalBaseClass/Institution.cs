using System;
using System.Collections.Generic;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    public interface IInstitutionRepository
    {
        List<Institution> GetInstitutionByID(string id);
        List<Institution> GetInstitutionByName(string name);
        List<Institution> GetInstitutionsByMultipleId(IEnumerable<string> institutionsID);
    }

    public class Institution
    {
        public bool IsCooperative { get; set; }
        public Institution()
        {

        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public DateTime EndContractDate { get; set; }
    }

    public enum InstitutionSearchCondition
    {
        ID,
        Name
    }

    public record InstitutionSearchPattern
    {
        public InstitutionSearchCondition Condition { get; set; }
        public string Value { get; set; }
        public InstitutionSearchPattern(InstitutionSearchCondition condition, string value)
        {
            Condition = condition;
            Value = value;
        }
    }
}
