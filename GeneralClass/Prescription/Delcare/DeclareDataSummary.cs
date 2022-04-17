using System.Collections.Generic;
using System.Linq;

namespace GeneralClass.Prescription.Delcare
{
    public readonly struct DeclareDataSummary
    {
        public DeclareDataSummary(IReadOnlyCollection<PrescriptionData> prescriptions)
        {
            NormalCount = prescriptions.Count(_ => _.IsNormalPrescription());
            ChronicCount = prescriptions.Count(_ => _.IsChronicPrescription());
            DayPayCount = prescriptions.Count(_ => _.IsDispenseByDayPrescription());
            DeclarePoint = prescriptions.Sum(_ => _.DeclarePoint);
            CopaymentPoint = prescriptions.Sum(_ => _.CopaymentPoint);
            TotalPoint = prescriptions.Sum(_ => _.TotalPoint);
        }
        public int NormalCount { get; }
        public int ChronicCount { get; }
        public int DayPayCount { get; }
        public int DeclarePoint { get; }
        public int CopaymentPoint { get; }
        public int TotalPoint { get; }
    }
}
