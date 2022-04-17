using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalOrders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Properties;

namespace GeneralClass.DeclareData
{
    [Serializable]
    [XmlRoot("ddata")]
    public class PointListSegment
    {
        public PointListSegment(){}
        public PointListSegment(PrescriptionData p)
        {
            Head = new PointListSegmentHead(p);
            Body = new PointListSegmentBody(p);
            var specialMaterialPoint = p.MedicalOrders.Where(m => m.Type == MedicalOrderType.SpecialMaterial).Sum(s => s.TotalPoint);
            Body.SpecialMaterialPoint = specialMaterialPoint.ToString("0000000");
            const int treatmentPoint = 0;
            Body.TreatmentPoint = "00000000";
            var medicinePoint = p.MedicalOrders.Where(m => m.Type == MedicalOrderType.Medicine).Sum(s => s.TotalPoint);
            Body.MedicinePoint = medicinePoint.ToString("00000000");
            var medicalServicePoint = p.MedicalOrders.Where(m => m.Type == MedicalOrderType.MedicalService).Sum(s => s.TotalPoint);
            Body.MedicalServicePoint = medicalServicePoint.ToString("00000000");
            var totalPoint = specialMaterialPoint + medicinePoint + medicalServicePoint + treatmentPoint;
            Head.TotalPoint = totalPoint.ToString("00000000");
            Head.ApplyPoint = (totalPoint - p.CopaymentPoint).ToString("00000000");
        }
        [XmlElement("dhead")]
        public PointListSegmentHead Head { get; set; }
        [XmlElement("dbody")]
        public PointListSegmentBody Body { get; set; }
    }

    [Serializable]
    [XmlRoot("dhead")]
    public class PointListSegmentHead
    {
        public PointListSegmentHead(){}
        public PointListSegmentHead(PrescriptionData p)
        {
            AdjustCase = p.AdjustCaseID.ID;
            SerialNumber = p.SerialNumber.ToString("000000");
            PatientId = p.Patient.IDNumber;
            PaymentCategory = PaymentCategoryService.GetPaymentCategoryById(p.PaymentCategoryID).ID.ID;
            Debug.Assert(p.Patient.Birthday != null, "p.Patient.Birthday != null");
            Birthday = ((DateTime)p.Patient.Birthday).ToChineseDateTimeString(DateTimeUnit.Day);
            MedicalSerialNumber = p.AdjustCaseID.ID == Resources.ChronicAdjustCaseID ? p.ChronicMedicalSerialNumber : p.MedicalSerialNum;
            DiseaseCode = p.MainDiseaseID.ID;
            SecondDiseaseCode = p.SubDiseaseID.ID;
            Division = p.DivisionID.ID;
            if(p.AdjustCaseID.ID == Resources.MedicalHomeCareAdjustCaseID)
                TreatmentDate = p.TreatDate.ToChineseDateTimeString(DateTimeUnit.Day);
            Copayment = p.CopaymentID.ID;
            CopaymentPoint = p.CopaymentPoint.ToString("0000");
            if (CopaymentService.GetCopaymentById(p.CopaymentID).IsAdministrativeAssistance)
                AdministrativeAssistCopaymentPoint = CopaymentPoint;
            PatientName = p.Patient.Name;
            InstitutionId = p.InstitutionID.ID;
            PrescriptionCase = p.PrescriptionCaseID.ID;
            AdjustDate = p.AdjustDate.ToChineseDateTimeString(DateTimeUnit.Day);
            DoctorIdNumber = InstitutionId;
            MedicalStaffId = p.Pharmacist.IDNumber;
        }
        [XmlElement("d1")]
        public string AdjustCase { get; set; }
        [XmlElement("d2")]
        public string SerialNumber { get; set; }
        [XmlElement("d3")]
        public string PatientId { get; set; }
        [XmlElement("d4")]
        public string RepayNote { get; set; }
        [XmlElement("d5")]
        public string PaymentCategory { get; set; }
        [XmlElement("d6")]
        public string Birthday { get; set; }
        [XmlElement("d7")]
        public string MedicalSerialNumber { get; set; }
        [XmlElement("d8")]
        public string DiseaseCode { get; set; }
        [XmlElement("d9")]
        public string SecondDiseaseCode { get; set; }
        [XmlElement("d13")]
        public string Division { get; set; }
        [XmlElement("d14")]
        public string TreatmentDate { get; set; }
        [XmlElement("d15")]
        public string Copayment { get; set; }
        [XmlElement("d16")]
        public string ApplyPoint { get; set; }
        [XmlElement("d17")]
        public string CopaymentPoint { get; set; }
        [XmlElement("d18")]
        public string TotalPoint { get; set; }
        [XmlElement("d19")]
        public string AdministrativeAssistCopaymentPoint { get; set; }
        [XmlElement("d20")]
        public string PatientName { get; set; }
        [XmlElement("d21")]
        public string InstitutionId { get; set; }
        [XmlElement("d22")]
        public string PrescriptionCase { get; set; }
        [XmlElement("d23")]
        public string AdjustDate { get; set; }
        [XmlElement("d24")]
        public string DoctorIdNumber { get; set; }
        [XmlElement("d25")]
        public string MedicalStaffId { get; set; }
    }

    [Serializable]
    [XmlRoot("dbody")]
    public class PointListSegmentBody
    {
        public PointListSegmentBody(){}
        public PointListSegmentBody(PrescriptionData p)
        {
            SpecialTreatment = p.SpecialTreatID.ID;
            MedicineDays = p.MedicineDays.ToString();
            if(p.AdjustCaseID.ID == Resources.ChronicAdjustCaseID)
            {
                ChronicAdjustSerialNumber = p.ChronicCurrentTimes.ToString();
                ChronicAdjustTimes = p.ChronicAvailableTimes.ToString();
                OriginalMedicalSerialNumber = p.MedicalSerialNum;
            }
            var medicalServiceOrder = p.MedicalOrders.Single(order => order.Type == MedicalOrderType.MedicalService);
            MedicalServiceCode = medicalServiceOrder.ID.ID;
            MedicalServicePoint = medicalServiceOrder.TotalPoint.ToString("00000000");
        }
        [XmlElement("d26")]
        public string SpecialTreatment { get; set; }
        [XmlElement("d30")]
        public string MedicineDays { get; set; }
        [XmlElement("d31")]
        public string SpecialMaterialPoint { get; set; }
        [XmlElement("d32")]
        public string TreatmentPoint { get; set; }
        [XmlElement("d33")]
        public string MedicinePoint { get; set; }
        [XmlElement("d35")]
        public string ChronicAdjustSerialNumber { get; set; }
        [XmlElement("d36")]
        public string ChronicAdjustTimes { get; set; }
        [XmlElement("d37")]
        public string MedicalServiceCode { get; set; }
        [XmlElement("d38")]
        public string MedicalServicePoint { get; set; }
        [XmlElement("d43")]
        public string OriginalMedicalSerialNumber { get; set; }
        [XmlElement("d44")]
        public string DependTreatmentNewbornBirthday { get; set; }
        [XmlElement("pdata")]
        public List<MedicalOrderListSegment> MedicalOrders { get; set; }
    }
}
