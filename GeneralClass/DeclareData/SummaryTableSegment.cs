using GeneralClass.Prescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using GeneralClass.Properties;

namespace GeneralClass.DeclareData
{
    public enum DeclareMethod
    {
        Zip,
        Web
    }
    public enum DeclareCategory
    {
        SendCheck,
        Repay
    }
    [Serializable]
    [XmlRoot("tdata")]
    public class SummaryTableSegment
    {
        public SummaryTableSegment()
        {

        }
        public SummaryTableSegment(List<PrescriptionData> prescriptions, DeclareMethod method, DeclareCategory category,List<PointListSegment> pointListSegments)
        {
            var normalCase = pointListSegments.Where(d => d.Head.AdjustCase != Resources.ChronicAdjustCaseID).ToArray();
            var chronicCase = pointListSegments.Where(d => d.Head.AdjustCase == Resources.ChronicAdjustCaseID).ToArray();
            ChargeYearMonth = prescriptions.First().AdjustDate.ToChineseDateTimeString(DateTimeUnit.Month);
            DeclareMethod = method == DeclareData.DeclareMethod.Zip ? Resources.DeclareMethodZip : Resources.DeclareMethodWeb;
            DeclareCategory = category == DeclareData.DeclareCategory.SendCheck ? Resources.DeclareCategorySendCheck : Resources.DeclareCategoryRepay;
            DeclareDate = DateTime.Today.ToChineseDateTimeString(DateTimeUnit.Day);
            NormalCaseCount = normalCase.Length;
            NormalCasePoint = normalCase.Sum(p => int.Parse(p.Head.ApplyPoint));
            ChronicCaseCount = chronicCase.Length;
            ChronicCasePoint = chronicCase.Sum(p => int.Parse(p.Head.ApplyPoint));
            TotalCaseCount = NormalCaseCount + ChronicCaseCount;
            TotalPoint = NormalCasePoint + ChronicCasePoint;
            var connectionStart = prescriptions.Min(p => p.AdjustDate);
            var connectionEnd = prescriptions.Max(p => p.AdjustDate);
            ConnectionStartDate = connectionStart.ToChineseDateTimeString(DateTimeUnit.Day);
            ConnectionEndDate = connectionEnd.ToChineseDateTimeString(DateTimeUnit.Day);
        }
        [XmlElement("t1")]
        public string DataFormat { get; set; } = Resources.DeclareFilePharmacyDataFormat;
        [XmlElement("t2")]
        public string PharmacyId { get; set; }
        [XmlElement("t3")]
        public string ChargeYearMonth { get; set; }
        [XmlElement("t4")]
        public string DeclareMethod { get; set; }
        [XmlElement("t5")]
        public string DeclareCategory { get; set; }
        [XmlElement("t6")]
        public string DeclareDate { get; set; }
        [XmlElement("t7")]
        public int NormalCaseCount { get; set; }
        [XmlElement("t8")]
        public int NormalCasePoint { get; set; }
        [XmlElement("t9")]
        public int ChronicCaseCount { get; set; }
        [XmlElement("t10")]
        public int ChronicCasePoint { get; set; }
        [XmlElement("t11")]
        public int TotalCaseCount { get; set; }
        [XmlElement("t12")]
        public int TotalPoint { get; set; }
        [XmlElement("t13")]
        public string ConnectionStartDate { get; set; }
        [XmlElement("t14")]
        public string ConnectionEndDate { get; set; }
    }
}
