using GeneralClass;
using GeneralClass.Customer;
using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalOrders;

namespace ReportingService
{
    public class MedicinesBagReport
    {
        public MedicinesBagReport(Customer patient,PrescriptionData prescription)
        {
            PatientName = patient.Name;
            Birthday = patient.Birthday?.ToChineseDateTimeString(DateTimeUnit.Day,"/");
            Gender = patient.Gender ? "男" : "女";
            //Institution = prescription.Institution.Name;
            //Division = prescription.Division.Name;
        }
        public string PatientName { get; set; }
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string Institution { get; set; }
        public string Division { get; set; }
    }

    public class Medicine
    {
        public Medicine(MedicalOrder order)
        {
            ID = order.ID.ID;
            ChineseName = order.ChineseName;
            EnglishName = order.EnglishName;
            Dosage = order.Dosage is not null ? $"每次{order.Dosage}" : string.Empty;
            Frequency = MedicalOrderService.GetFrequency(order.Frequency,order.Days ?? 0).Description;
            ActionSite = MedicalOrderService.GetActionSiteDescription(order.ActionSite);
            Days = order.Days is null ? string.Empty : $"{order.Days}天";
            TotalAmount = order.TotalAmount;
        }
        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string ActionSite { get; set; }
        public string Days { get; set; }
        public float TotalAmount { get; set; }
    }
}