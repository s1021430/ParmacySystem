using System;
using System.Collections.Generic;
using System.Linq;
using GeneralClass.Customer.EntityIndex;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Prescription.MedicalOrders;

namespace GeneralClass.Prescription
{
    public class PrescriptionData
    {
        public PrescriptionID ID { get; set; }
        public InstitutionID InstitutionID { get; set; }
        public DivisionID DivisionID { get;set; }
        public AdjustCaseID AdjustCaseID { get; set; }
        public CopaymentID CopaymentID { get; set; }
        public PrescriptionCaseID PrescriptionCaseID { get; set; }
        public string PharmacistID { get; set; }
        public Employee.Employee Pharmacist { get; set; }
        public CustomerID PatientID { get; set; }
        public Customer.Customer Patient { get;set; }
        public string MedicalSerialNum { get; set; }
        public DateTime TreatDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public DiseaseCodeID MainDiseaseID { get; set; }
        public DiseaseCodeID SubDiseaseID { get; set; }
        public PaymentCategoryID PaymentCategoryID { get; set; }
        public SpecialTreatID SpecialTreatID { get; set; }
        public List<MedicalOrder> MedicalOrders { get; set; } = new(); //醫令
        public int MedicinePoint { get; set; }
        public int SpecialMaterialPoint { get; set; }
        public int CopaymentPoint { get; set; }
        public int MedicalServicePoint { get; set; }
        public int DeclarePoint { get; set; }
        public int TotalPoint { get; set; }
        public int MedicineSelfPayAmount { get; set; }
        public int ChronicAvailableTimes { get; set; }//慢箋可調劑次數
        public int ChronicCurrentTimes { get; set; }//慢箋當前調劑次數
        public int MedicineDays { get; set; }//給藥天數
        public string ChronicMedicalSerialNumber { get; set; }
        public int SerialNumber { get; set; }
        public int WareHouseID { get; set; }

        public void CreateMedicalServiceOrder()
        {
            var serviceOrder = new MedicalServiceOrder(MedicineDays, AdjustDate);
            serviceOrder.CountTotalPoint();
            if (MedicalOrders is null)
                MedicalOrders = new List<MedicalOrder>{ serviceOrder };
            else
                MedicalOrders.Add(serviceOrder);
        }
        public PrescriptionData(){}
        public PrescriptionData(PrescriptionID id)
        {
            ID = id;
        }
        
        public void CalculateMedicinePoint()
        {
            if(MedicalOrders is null || MedicalOrders.Count(order => order.Type == MedicalOrderType.Medicine) <= 0) 
                MedicinePoint = 0;
            else
                MedicinePoint = MedicalOrders.Where(order => order.Type == MedicalOrderType.Medicine && !order.OwnExpense).Sum(order => order.TotalPoint);
        }

        public void CalculateSpecialMaterialPoint()
        {
            if (MedicalOrders is null || MedicalOrders.Count(order => order.Type == MedicalOrderType.SpecialMaterial) <= 0)
                SpecialMaterialPoint = 0;
            else
                SpecialMaterialPoint = MedicalOrders.Where(order => order.Type == MedicalOrderType.SpecialMaterial && !order.OwnExpense).Sum(order => order.TotalPoint);
        }

        public void CalculateMedicalServicePoint()
        {
            if (MedicalOrders is null || MedicalOrders.Count(order => order.Type == MedicalOrderType.MedicalService) <= 0)
                MedicalServicePoint = 0;
            else
                MedicalServicePoint = MedicalOrders.Where(order => order.Type == MedicalOrderType.MedicalService).Sum(order => order.TotalPoint);
        }

        public void CalculateDeclarePoint()
        {
            DeclarePoint = TotalPoint - CopaymentPoint;
        }

        public void CalculateTotalPoint()
        {
            TotalPoint = SpecialMaterialPoint + MedicinePoint + MedicalServicePoint + CopaymentPoint;
        }

        public void CalculateCopaymentPoint()
        {
            var copayment = CopaymentService.GetCopaymentById(CopaymentID);
            CopaymentPoint = CopaymentService.CountCopaymentPoint(copayment, MedicinePoint);
        }

        public void CalculatePoints()
        {
            CalculateMedicinePoint();
            CalculateCopaymentPoint();
            CalculateSpecialMaterialPoint();
            CalculateMedicalServicePoint();
            CalculateTotalPoint();
            CalculateDeclarePoint();
        }

        public bool IsNormalPrescription()
        {
            return AdjustCaseID.ID.Equals(Properties.Resources.NormalAdjustCaseID);
        }

        public bool IsChronicPrescription()
        {
            return AdjustCaseID.ID.Equals(Properties.Resources.ChronicAdjustCaseID);
        }
        public bool IsDispenseByDayPrescription()
        {
            return AdjustCaseID.ID.Equals(Properties.Resources.DispenseByDayAdjustCaseID);
        }
    }
}
