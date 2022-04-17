using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    public readonly struct Division
    {
        public Division(DivisionID divisionID, string name)
        {
            DivisionID = divisionID;
            Name = name;
        }
        public DivisionID DivisionID { get; }
        public string Name { get; }
    }

    public static class DivisionRepository
    {
        private static readonly List<Division> DivisionList = new()
        {
            new Division(new DivisionID("00"), "不分科"),
            new Division(new DivisionID("01"), "家醫科"),
            new Division(new DivisionID("02"), "內科"),
            new Division(new DivisionID("03"), "外科"),
            new Division(new DivisionID("04"), "兒科"),
            new Division(new DivisionID("05"), "婦產科"),
            new Division(new DivisionID("06"), "骨科"),
            new Division(new DivisionID("07"), "神經外科"),
            new Division(new DivisionID("08"), "泌尿科"),
            new Division(new DivisionID("09"), "耳鼻喉科"),
            new Division(new DivisionID("10"), "眼科"),
            new Division(new DivisionID("11"), "皮膚科"),
            new Division(new DivisionID("12"), "神經科"),
            new Division(new DivisionID("13"), "精神科"),
            new Division(new DivisionID("14"), "復健科"),
            new Division(new DivisionID("15"), "整形外科"),
            new Division(new DivisionID("16"), "職業醫學科"),
            new Division(new DivisionID("22"), "急診醫學科"),
            new Division(new DivisionID("2A"), "結核科"),
            new Division(new DivisionID("2B"), "洗腎科"),
            new Division(new DivisionID("40"), "牙科"),
            new Division(new DivisionID("41"), "復形牙科"),
            new Division(new DivisionID("42"), "牙髓病科"),
            new Division(new DivisionID("43"), "牙周病科"),
            new Division(new DivisionID("44"), "補綴牙科"),
            new Division(new DivisionID("45"), "齒顎矯正科"),
            new Division(new DivisionID("46"), "兒童牙科"),
            new Division(new DivisionID("47"), "口腔顎面外科"),
            new Division(new DivisionID("48"), "口腔診斷科"),
            new Division(new DivisionID("49"), "口腔病理科"),
            new Division(new DivisionID("60"), "中醫一般科"),
            new Division(new DivisionID("81"), "麻醉科"),
            new Division(new DivisionID("82"), "放射線科"),
            new Division(new DivisionID("83"), "病理科"),
            new Division(new DivisionID("84"), "核子醫學科"),
            new Division(new DivisionID("85"), "放射腫瘤科"),
            new Division(new DivisionID("86"), "放射診斷科"),
            new Division(new DivisionID("87"), "解剖病理科"),
            new Division(new DivisionID("88"), "臨床病理科")
        };

        internal static List<Division> GetDivisionList()
        {
            return DivisionList;
        }
    }

    public static class DivisionService
    {
        public static List<Division> GetDivisionList()
        {
            return DivisionRepository.GetDivisionList();
        }

        public static bool IsExist(string id)
        {
            return GetDivisionList().Select(_ => _.DivisionID.ID).Contains(id);
        }

        public static bool IsDentistry(Division division)
        {
            if (string.IsNullOrEmpty(division.DivisionID.ID)) return false;
            var dentistryList = new List<string> {"40", "41", "42", "43", "44", "45", "46"};
            return dentistryList.Contains(division.DivisionID.ID);
        }

        public static Division GetDivisionById(DivisionID id)
        {
            return GetDivisionList().SingleOrDefault(d => d.DivisionID.Equals(id));
        }
    }
}