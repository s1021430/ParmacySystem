using GeneralClass.Prescription;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneralClass.DeclareData
{
    [Serializable]
    [XmlRoot("pharmacy")]
    public class DeclareFile
    {
        public DeclareFile()
        {

        }
        public DeclareFile(List<PrescriptionData> prescriptions, DeclareMethod method, DeclareCategory category)
        {
            PointListSegments = new List<PointListSegment>();
            foreach (var p in prescriptions)
                PointListSegments.Add(new PointListSegment(p));
            SummaryTableSegment = new SummaryTableSegment(prescriptions, method, category, PointListSegments);
        }

        [XmlElement("tdata")]
        public SummaryTableSegment SummaryTableSegment { get; set; }
        [XmlElement("ddata")]
        public List<PointListSegment> PointListSegments { get; set; }
    }
}
