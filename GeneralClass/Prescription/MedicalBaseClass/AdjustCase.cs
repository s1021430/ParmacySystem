using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Properties;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    public readonly struct AdjustCase
    {
        public AdjustCase(AdjustCaseID adjustCaseID,string name)
        {
            AdjustCaseID = adjustCaseID;
            Name = name;
        }

        public AdjustCaseID AdjustCaseID { get; }
        public string Name { get; }
    }

    public static class AdjustCaseRepository
    {
        private static readonly List<AdjustCase> AdjustCaseList = new()
        {
            new AdjustCase(new AdjustCaseID(string.Empty), string.Empty),
            new AdjustCase(new AdjustCaseID("0"),"自費調劑"),
            new AdjustCase(new AdjustCaseID("1"),"一般處方"),
            new AdjustCase(new AdjustCaseID("2"),"慢性病連續處方"),
            new AdjustCase(new AdjustCaseID("3"),"日劑藥費"),
            new AdjustCase(new AdjustCaseID("5"),"協助辦理門診戒菸計畫"),
            new AdjustCase(new AdjustCaseID("D"),"藥事居家照護"),
            new AdjustCase(new AdjustCaseID("E"),"居家醫療照護整合計畫")
        };

        internal static List<AdjustCase> GetAdjustCaseList()
        {
            return AdjustCaseList;
        }
    }

    public static class AdjustCaseService
    {
        public static List<AdjustCase> GetAdjustCaseList()
        {
            return AdjustCaseRepository.GetAdjustCaseList();
        }

        public static bool IsExist(AdjustCaseID id)
        {
            return GetAdjustCaseList().Exists(a => a.AdjustCaseID.Equals(id));
        }

        public static bool IsChronic(AdjustCase adjustCase)
        {
            var adjustCaseID = adjustCase.AdjustCaseID.ID;
            if (string.IsNullOrEmpty(adjustCaseID)) return false;
            return adjustCaseID == Resources.ChronicAdjustCaseID;
        }

        public static bool IsOwnExpense(AdjustCase adjustCase)
        {
            var adjustCaseID = adjustCase.AdjustCaseID.ID;
            if (string.IsNullOrEmpty(adjustCaseID)) return false;
            return adjustCaseID == Resources.OwnExpenseAdjustCaseID;
        }

        public static AdjustCase GetAdjustCaseById(AdjustCaseID id)
        {
            return GetAdjustCaseList().SingleOrDefault(adjustCase => adjustCase.AdjustCaseID.Equals(id));
        }
    }
}
