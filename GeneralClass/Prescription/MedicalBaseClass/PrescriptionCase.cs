using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    public readonly struct PrescriptionCase
    {
        public PrescriptionCase(PrescriptionCaseID prescriptionCaseID, string name)
        {
            PrescriptionCaseID = prescriptionCaseID;
            Name = name;
        }

        public PrescriptionCaseID PrescriptionCaseID { get; }
        public string Name { get; }
    }

    public static class PrescriptionCaseRepository
    {
        private static readonly List<PrescriptionCase> PrescriptionCaseList = new()
        {
            new PrescriptionCase(new PrescriptionCaseID(string.Empty),string.Empty),
            new PrescriptionCase(new PrescriptionCaseID("01"),"西醫一般案件"),
            new PrescriptionCase(new PrescriptionCaseID("02"),"西醫急診"),
            new PrescriptionCase(new PrescriptionCaseID("03"),"西醫門診手術"),
            new PrescriptionCase(new PrescriptionCaseID("04"),"西醫慢性病"),
            new PrescriptionCase(new PrescriptionCaseID("05"),"洗腎"),
            new PrescriptionCase(new PrescriptionCaseID("06"),"結核病"),
            new PrescriptionCase(new PrescriptionCaseID("07"),"遠距醫療"),
            new PrescriptionCase(new PrescriptionCaseID("08"),"慢性病連續處方調劑"),
            new PrescriptionCase(new PrescriptionCaseID("09"),"西醫其他專案"),
            new PrescriptionCase(new PrescriptionCaseID("11"),"牙醫一般案件"),
            new PrescriptionCase(new PrescriptionCaseID("12"),"牙醫急診"),
            new PrescriptionCase(new PrescriptionCaseID("13"),"牙醫門診手術"),
            new PrescriptionCase(new PrescriptionCaseID("14"),"牙醫門診總額醫療資源不足地區改善方案"),
            new PrescriptionCase(new PrescriptionCaseID("16"),"牙醫特殊專案醫療服務項目"),
            new PrescriptionCase(new PrescriptionCaseID("19"),"牙醫其他專案"),
            new PrescriptionCase(new PrescriptionCaseID("21"),"中醫一般案件"),
            new PrescriptionCase(new PrescriptionCaseID("22"),"中醫其他專案"),
            new PrescriptionCase(new PrescriptionCaseID("23"),"中醫現代科技加強醫療服務方案"),
            new PrescriptionCase(new PrescriptionCaseID("24"),"中醫慢性病"),
            new PrescriptionCase(new PrescriptionCaseID("25"),"中醫門診總額醫療資源不足地區改善方案"),
            new PrescriptionCase(new PrescriptionCaseID("28"),"中醫慢性病連續處方調劑"),
            new PrescriptionCase(new PrescriptionCaseID("29"),"中醫針灸、傷科及脫臼整復"),
            new PrescriptionCase(new PrescriptionCaseID("30"),"中醫特定疾病門診加強照護"),
            new PrescriptionCase(new PrescriptionCaseID("31"),"中醫居家"),
            new PrescriptionCase(new PrescriptionCaseID("A1"),"居家照護"),
            new PrescriptionCase(new PrescriptionCaseID("A2"),"精神疾病社區復健"),
            new PrescriptionCase(new PrescriptionCaseID("A3"),"預防保健 "),
            new PrescriptionCase(new PrescriptionCaseID("A5")," 安寧居家療護"),
            new PrescriptionCase(new PrescriptionCaseID("A6"),"護理之家居家照護"),
            new PrescriptionCase(new PrescriptionCaseID("A7"),"安養、養護機構院民之居家照護"),
            new PrescriptionCase(new PrescriptionCaseID("B1"),"行政協助性病患者全面篩檢愛滋病毒計畫"),
            new PrescriptionCase(new PrescriptionCaseID("B6"),"職災案件"),
            new PrescriptionCase(new PrescriptionCaseID("B7"),"行政協助門診戒菸"),
            new PrescriptionCase(new PrescriptionCaseID("B8"),"行政協助精神科強制住院"),
            new PrescriptionCase(new PrescriptionCaseID("B9"),"行政協助孕婦全面篩檢愛滋計畫"),
            new PrescriptionCase(new PrescriptionCaseID("BA"),"愛滋防治治療替代計畫"),
            new PrescriptionCase(new PrescriptionCaseID("C1"),"論病例計酬案件"),
            new PrescriptionCase(new PrescriptionCaseID("C4"),"行政協助無健保結核病患就醫案件"),
            new PrescriptionCase(new PrescriptionCaseID("D1"),"行政協助愛滋病案件"),
            new PrescriptionCase(new PrescriptionCaseID("D2"),"行政協助兒童常規疫苗、流感疫苗及 75歲以上長者肺炎鏈球菌疫苗接種"),
            new PrescriptionCase(new PrescriptionCaseID("D4"),"西醫基層(醫院支援)醫療資源不足地區改善方案"),
            new PrescriptionCase(new PrescriptionCaseID("E1"),"醫療給付改善方案及試辦計畫"),
            new PrescriptionCase(new PrescriptionCaseID("E2"),"愛滋病確診服藥滿2年後案件"),
            new PrescriptionCase(new PrescriptionCaseID("E3"),"愛滋病確診服藥滿2年後案件之慢性病連續處方再調劑"),
            new PrescriptionCase(new PrescriptionCaseID("DF"),"代辦登革熱NS1抗原快速篩檢試劑")
        };

        internal static List<PrescriptionCase> GetPrescriptionCaseList()
        {
            return PrescriptionCaseList;
        }
    }

    public static class PrescriptionCaseService
    {
        public static List<PrescriptionCase> GetPrescriptionCaseList()
        {
            return PrescriptionCaseRepository.GetPrescriptionCaseList();
        }

        public static bool IsExist(PrescriptionCaseID id)
        {
            return GetPrescriptionCaseList().Exists(p => p.PrescriptionCaseID.Equals(id));
        }

        public static PrescriptionCase GetPrescriptionCaseById(PrescriptionCaseID id)
        {
            return GetPrescriptionCaseList().SingleOrDefault(p => p.PrescriptionCaseID.Equals(id));
        }
    }
}