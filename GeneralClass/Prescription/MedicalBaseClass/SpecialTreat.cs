using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    /*
     * H1:全民健康保險加強慢性 B、C型肝炎治療計畫(93.1增訂)
       H2:西醫-行動不便者，經醫師認定或經受託人提供切結文件，慢性病代領藥案件(96.7 增訂；101.11 文字修訂) 
       H3:西醫-已出海為遠洋漁船作業船員，提供切結文件，慢性病代領藥案件(96.7 增訂；101.11 文字修訂) 
       H4:自費健檢發現病兆加作處置或檢查（97.1 增訂）
       H6:西醫-已出海為國際航線船舶作業船員，提供切結文件，慢性病代領藥案件(97.10 增訂；101.11 文字修訂) 
       H7:全民健康保險 B 型肝炎帶原者及 C 型肝炎感染者醫療給付改善方案(99.1 增訂)28
       H8:西醫-持慢性病連續處方箋領藥，預定出國，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 文字修訂）。
       H9:西醫-經保險人認定之特殊情形，慢性病代領藥案件（101.11 新增）。
       HA:西醫-持慢性病連續處方箋領藥，返回離島地區，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 新增）。
       HB:西醫-持慢性病連續處方箋領藥，已出海為遠洋漁船作業船員，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 新增）。
       HC:西醫-持慢性病連續處方箋領藥，已出海為國際航線船舶作業船員，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 新增）。
       HD:西醫-持慢性病連續處方箋領藥，罕見疾病病人，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 新增）。
       HE:C 型肝炎全口服治療(106.1.24 新增)
       HF:慢性阻塞性肺病醫療給付改善方案(106.04.01 新增)
       HG:西醫-受監護或輔助宣告，經受託人提供法院裁定文件影本(107.04.27 新增)
       HH:西醫-經醫師認定之失智症病人(107.04.27 新增)
       HI:西醫-經保險人認定確有一次領取該處方箋總用藥量必要之特殊病人(107.04.27 新增)
     */
    public readonly struct SpecialTreat
    {
        public SpecialTreat(SpecialTreatID id, string name)
        {
            ID = id;
            Name = name;
        }

        public SpecialTreatID ID { get; }
        public string Name { get; }
    }

    public class SpecialTreatRepository
    {
        private static readonly List<SpecialTreat> SpecialTreatList = new()
        {
            new SpecialTreat(new SpecialTreatID(string.Empty), string.Empty),
            new SpecialTreat(new SpecialTreatID("H1"),"全民健康保險加強慢性B、C型肝炎治療計畫"),
            new SpecialTreat(new SpecialTreatID("H2"),"西醫-行動不便者，經醫師認定或經受託人提供切結文件，慢性病代領藥案件"),
            new SpecialTreat(new SpecialTreatID("H3"),"西醫-已出海為遠洋漁船作業船員，提供切結文件，慢性病代領藥案件"),
            new SpecialTreat(new SpecialTreatID("H4"),"自費健檢發現病兆加作處置或檢查"),
            new SpecialTreat(new SpecialTreatID("H6"),"西醫-已出海為國際航線船舶作業船員，提供切結文件，慢性病代領藥案件"),
            new SpecialTreat(new SpecialTreatID("H7"),"全民健康保險 B 型肝炎帶原者及 C 型肝炎感染者醫療給付改善方案"),
            new SpecialTreat(new SpecialTreatID("H8"),"西醫-持慢性病連續處方箋領藥，預定出國，提供切結文件，一次領取2個月或3個月用藥量案件"),
            new SpecialTreat(new SpecialTreatID("H9"),"西醫-經保險人認定之特殊情形，慢性病代領藥案件"),
            new SpecialTreat(new SpecialTreatID("HA"),"西醫-持慢性病連續處方箋領藥，返回離島地區，提供切結文件，一次領取2個月或3個月用藥量案件"),
            new SpecialTreat(new SpecialTreatID("HB"),"西醫-持慢性病連續處方箋領藥，已出海為遠洋漁船作業船員，提供切結文件，一次領取2個月或3個月用藥量案件"),
            new SpecialTreat(new SpecialTreatID("HC"),"西醫-持慢性病連續處方箋領藥，已出海為國際航線船舶作業船員，提供切結文件，一次領取2個月或3個月用藥量案件"),
            new SpecialTreat(new SpecialTreatID("HD"),"西醫-持慢性病連續處方箋領藥，罕見疾病病人，提供切結文件，一次領取 2 個月或 3 個月用藥量案件"),
            new SpecialTreat(new SpecialTreatID("HE"),"C 型肝炎全口服治療"),
            new SpecialTreat(new SpecialTreatID("HF"),"慢性阻塞性肺病醫療給付改善方案"),
            new SpecialTreat(new SpecialTreatID("HG"),"西醫-受監護或輔助宣告，經受託人提供法院裁定文件影本"),
            new SpecialTreat(new SpecialTreatID("HH"),"西醫-經醫師認定之失智症病人"),
            new SpecialTreat(new SpecialTreatID("HI"),"西醫-經保險人認定確有一次領取該處方箋總用藥量必要之特殊病人")
        };

        internal static List<SpecialTreat> GetSpecialTreatList()
        {
            return SpecialTreatList;
        }
    }

    public static class SpecialTreatService
    {
        public static List<SpecialTreat> GetSpecialTreatList()
        {
            return SpecialTreatRepository.GetSpecialTreatList();
        }

        public static bool IsSpecialTreatExist(string specialTreatId)
        {
            return GetSpecialTreatList().Any(_ => _.ID.Equals(specialTreatId));
        }

        public static SpecialTreat GetSpecialTreatByID(SpecialTreatID id)
        {
            return GetSpecialTreatList().SingleOrDefault(specialTreat => specialTreat.ID.Equals(id));
        }
    }
}
