using GeneralClass.Prescription.MedicalOrders;
using System;
using System.Xml.Serialization;

namespace GeneralClass.DeclareData
{
    [Serializable]
    [XmlRoot("pdata")]
    public class MedicalOrderListSegment
    {
        public MedicalOrderListSegment(){}
        public MedicalOrderListSegment(MedicineOrder medicine, int serialNumber)
        {
            Type = MedicalOrderService.GetTypeId(medicine.Type);
            Id = medicine.ID.ID;
            TotalAmount = medicine.TotalAmount.ToString("00000.0");
            TotalAmount =
            UnitPrice = medicine.Price.ToString("0000000.00");
            TotalPoint = medicine.TotalPoint.ToString("00000000");
            Dosage = medicine.Dosage?.ToString("0000.00");
            Frequency = medicine.Frequency;
            ActionSite = medicine.ActionSite;
            AdditionRatio = (medicine.AdditionRatio * 100)?.ToString("000");
            SerialNumber = serialNumber.ToString();
            Days = medicine.Days.ToString();
            ExecutionStartDate = medicine.ExecutionStart.ToChineseDateTimeString(DateTimeUnit.Minute);
            ExecutionStartEnd = medicine.ExecutionEnd.ToChineseDateTimeString(DateTimeUnit.Minute);
            BatchNumber = medicine.BatchNumber;
        }
        public MedicalOrderListSegment(SpecialMaterialOrder specialMaterial, int serialNumber)
        {
            Type = MedicalOrderService.GetTypeId(specialMaterial.Type);
            Id = specialMaterial.ID.ID;
            TotalAmount = specialMaterial.TotalAmount.ToString("00000.0");
            UnitPrice = specialMaterial.Price.ToString("0000000.00");
            TotalPoint = specialMaterial.TotalPoint.ToString("00000000");
            Dosage = specialMaterial.Dosage?.ToString("0000.00");
            if (!string.IsNullOrEmpty(specialMaterial.Frequency))
                Frequency = specialMaterial.Frequency;
            if (!string.IsNullOrEmpty(specialMaterial.ActionSite))
                ActionSite = specialMaterial.ActionSite;
            AdditionRatio = (specialMaterial.AdditionRatio * 100)?.ToString("000");
            SerialNumber = serialNumber.ToString();
            Days = specialMaterial.Days?.ToString();
            ExecutionStartDate = specialMaterial.ExecutionStart.ToChineseDateTimeString(DateTimeUnit.Minute);
            ExecutionStartEnd = specialMaterial.ExecutionEnd.ToChineseDateTimeString(DateTimeUnit.Minute);
        }
        public MedicalOrderListSegment(VirtualOrder virtualOrder, int serialNumber)
        {
            Type = MedicalOrderService.GetTypeId(virtualOrder.Type);
            Id = virtualOrder.ID.ID;
            TotalAmount = virtualOrder.TotalAmount.ToString("00000.0");
            UnitPrice = virtualOrder.Price.ToString("0000000.00");
            TotalPoint = virtualOrder.TotalPoint.ToString("00000000");
            Dosage = virtualOrder.Dosage?.ToString("0000.00");
            SerialNumber = serialNumber.ToString();
            ExecutionStartDate = virtualOrder.ExecutionStart.ToChineseDateTimeString(DateTimeUnit.Minute);
            ExecutionStartEnd = virtualOrder.ExecutionEnd.ToChineseDateTimeString(DateTimeUnit.Minute);
        }
        [XmlElement("p1")]
        public string Type { get; set; }
        [XmlElement("p2")]
        public string Id { get; set; }
        [XmlElement("p7")]
        public string TotalAmount { get; set; }
        [XmlElement("p8")]
        public string UnitPrice { get; set; }
        [XmlElement("p9")]
        public string TotalPoint { get; set; }
        [XmlElement("p3")]
        public string Dosage { get; set; }
        [XmlElement("p4")]
        public string Frequency { get; set; }
        [XmlElement("p5")]
        public string ActionSite { get; set; }
        [XmlElement("p6")]
        public string AdditionRatio { get; set; }
        [XmlElement("p10")]
        public string SerialNumber { get; set; }
        [XmlElement("p11")]
        public string Days { get; set; }
        [XmlElement("p12")]
        public string ExecutionStartDate { get; set; }
        [XmlElement("p13")]
        public string ExecutionStartEnd { get; set; }
        [XmlElement("p15")]
        public string OwnExpenseSpecialMaterialGroupOrder { get; set; }
        [XmlElement("p16")]
        public string BatchNumber { get; set; }
    }
}
