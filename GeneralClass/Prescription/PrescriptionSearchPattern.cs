using System;

namespace GeneralClass.Prescription
{
    public record PrescriptionSearchPattern
    {
        public PrescriptionSearchPattern()
        {

        }
        public DateTime StartDate;
        public DateTime EndDate;
        public string InstitutionId;
        public string PatientIdNumber;
        public string PatientName;
        public DateTime? PatientBirthday;
        public string AdjustCaseId;
        public int? PharmacistId;
    }
}
