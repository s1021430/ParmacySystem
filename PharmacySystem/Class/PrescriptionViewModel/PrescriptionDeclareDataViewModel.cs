using System;
using GalaSoft.MvvmLight;
using GeneralClass.Employee;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalBaseClass;

namespace PharmacySystem.Class.PrescriptionViewModel
{
    public class PrescriptionDeclareDataViewModel : ObservableObject
    {
        public PrescriptionDeclareDataViewModel()
        {

        }

        public PrescriptionDeclareDataViewModel(PrescriptionData data)
        {
            ID = data.ID;
            Division = DivisionService.GetDivisionById(data.DivisionID);
            AdjustCase = AdjustCaseService.GetAdjustCaseById(data.AdjustCaseID);
            AdjustDate = data.AdjustDate;
            TreatDate = data.TreatDate;
            MedicalSerialNum = data.MedicalSerialNum;
            CopaymentPoint = data.CopaymentPoint;
            DeclarePoint = data.DeclarePoint;
            MedicalServicePoint = data.MedicalServicePoint;
            SpecialMaterialPoint = data.SpecialMaterialPoint;
            MedicinePoint = data.MedicinePoint;
            TotalPoint = data.TotalPoint;
        }

        public PrescriptionID ID;
        private bool declare = true;
        public bool Declare
        {
            get => declare;
            set
            {
                Set(() => Declare, ref declare, value);
            }
        }
        public PatientDataViewModel Patient { get; set; }
        public Institution Institution { get; set; }
        public Division Division { get; }
        public AdjustCase AdjustCase { get; }
        public Employee Pharmacist { get; set; }
        public string MedicalSerialNum { get; }
        public DateTime TreatDate { get; }
        public DateTime AdjustDate { get; }
        public int MedicinePoint { get; }
        public int SpecialMaterialPoint { get; }
        public int CopaymentPoint { get; }
        public int MedicalServicePoint { get; }
        public int DeclarePoint { get; }
        public int TotalPoint { get; }
    }
}
