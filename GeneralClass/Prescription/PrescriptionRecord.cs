using System;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalBaseClass;

namespace GeneralClass.Prescription
{
    public class PrescriptionRecord
    {
        public PrescriptionRecord(){}
        public readonly PrescriptionID ID;
        public string PatientName { get; }
        public DateTime? PatientBirthday { get; }
        public string PatientIdNumber { get; }
        public string Institution { get; }
        public DivisionID DivisionID { get; }
        public Division Division { get; set; }
        public AdjustCaseID AdjustCaseID { get; }
        public AdjustCase AdjustCase { get; set; }
        public DateTime TreatDate { get; }
        public DateTime AdjustDate { get; }
        public string MedicalSerialNum { get; }
        public string Pharmacist { get; }
    }
}
