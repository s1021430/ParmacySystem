using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Properties;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    public readonly struct Copayment
    {
        public Copayment(CopaymentID copaymentID, string name, bool isFree, bool isAdministrativeAssistance = false)
        {
            CopaymentID = copaymentID;
            Name = name;
            IsFree = isFree;
            IsAdministrativeAssistance = isAdministrativeAssistance;
        }
        public CopaymentID CopaymentID { get; }
        public string Name { get; }

        public readonly bool IsFree;

        public readonly bool IsAdministrativeAssistance;
    }

    /*一、006，001~009(除006)，801，802，901，902，903，904者依序優先填寫，其餘下列規定
    *部分負擔代碼填寫。
    *二、資料格式30(藥局)部分負擔代碼：
    *I20:須加收藥費部分負擔者。
    *I21:藥費一百元以下免加收藥費部分負擔者
    *I22:符合本保險藥費免部分負擔範圍規定者，包括慢性病連續處方箋案件、牙醫案件、
    *門診論病例計酬案件。
    *Z00:戒菸服務補助計畫須加收部分負擔者。
    *K00:居家照護(108.6增訂)
    *藥事居家照護（案件分類D）者，本欄請填“009”。
    * 註 5：免部分負擔代碼規定：
    *  ‧代碼 001:重大傷病
    *  ‧代碼 002:分娩
    *  ‧代碼 003:合於社會救助法規定之低收入戶之保險對象(第五類之保險對象)(協助衛生福利部辦理項目)
    *  18
    *  ‧代碼 004:榮民、榮民遺眷之家戶代表(第六類第一目之保險對象) (協助國軍退除役官兵輔導委員會辦理項目)
    *  ‧代碼 005:經登記列管結核病患至衛生福利部疾病管制署公告指定之醫療院所就醫者(協助疾病管制署辦理項目)
    *  ‧代碼 006:勞工保險被保險人因職業傷害或職業病門診者(協助勞工保險局辦理項目)
    *  ‧代碼 007:山地離島地區之就醫（88.7 增訂）、山地原住民暨離島地區接受醫療院所戒菸治療服務免除戒菸藥品部分負擔
    *  ‧代碼 008:經離島醫院診所轉診至台灣本島門診及急診就醫者（僅當次轉診適用）
    *  ‧代碼 009:本署其他規定免部分負擔者，如產檢時，同一主治醫師併同開給一般處方，百歲人瑞免部分負擔，921 震災，行政協
    *  助性病或藥癮病患全面篩檢愛滋計畫、行政協助孕婦全面篩檢愛滋計畫、八仙樂園粉塵暴燃事件
    *  (104.06.27~104.09.30)、有職災單之非職災醫療費用改健保支付(105.11.01 新增)、西醫就診 92093B 另以門診牙醫申報
    *  (106.11.01 新增)等
    *  ‧代碼 801:HMO 巡迴醫療
    *  ‧代碼 802:蘭綠計畫
    *  ‧代碼 901:多氯聯苯中毒之油症患者(協助國民健康署辦理項目)
    *  ‧代碼 902:三歲以下兒童醫療補助計畫(91.03.1 增訂) (協助衛生福利部辦理項目)
    *  ‧代碼 903:新生兒依附註記方式就醫者(92.9 增訂) (協助衛生福利部辦理項目)
    *  ‧代碼 904:行政協助愛滋病案件(95.3 增訂)、愛滋防治替代治療計畫(協助疾病管制署辦理項目)
    *  ‧代碼 905:三氯氰胺污染奶製品案(97.09.23 增訂，限門診適用)
    *  ‧代碼 906:內政部役政署補助替代役役男全民健康保險自行負擔醫療費用（102.01.01 起適用；協助內政部役政署辦理項目）
    *  ‧代碼 907:原住民於非山地暨離島地區接受戒菸服務者【104.11.01(含)起增訂】
    *  ‧代碼 908：代辦海洋委員會海巡署補助部分負擔
    *  ‧代碼 909：代辦中央警察大學補助部分負擔
    *  ‧代碼 910：代辦內政部消防署補助部分負擔
    *  ‧代碼 911：代辦內政部空勤總隊補助部分負擔
    *  ‧代碼 912：代辦內政部警政署補助部分負擔
    *  ‧代碼 913：代辦國防部補助部分負擔
    */
    public static class CopaymentRepository
    {
        private static readonly List<Copayment> CopaymentList = new()
        {
            new Copayment(new CopaymentID(string.Empty), string.Empty, true),
            new Copayment(new CopaymentID("I20"),"加收部分負擔",false),
            new Copayment(new CopaymentID("I21"),"藥費一百元以下免加",true),
            new Copayment(new CopaymentID("I22"),"免收(慢箋、牙醫、門診論病計酬)",true),
            new Copayment(new CopaymentID("Z00"),"戒菸加收部分負擔",false),
            new Copayment(new CopaymentID("K00"),"居家照護",false),
            new Copayment(new CopaymentID("001"),"重大傷病",true),
            new Copayment(new CopaymentID("002"),"分娩",true),
            new Copayment(new CopaymentID("003"),"低收入戶",true,true),
            new Copayment(new CopaymentID("004"),"榮民",true,true),
            new Copayment(new CopaymentID("005"),"結核病患至指定之醫療院所就醫",true,true),
            new Copayment(new CopaymentID("006"),"勞工保險被保險人因職業傷害或職業病門診者",true,true),
            new Copayment(new CopaymentID("007"),"山地離島就醫/戒菸免收",true),
            new Copayment(new CopaymentID("008"),"經離島醫院診所轉至台灣本門及急救",true),
            new Copayment(new CopaymentID("009"),"其他免負擔",true),
            new Copayment(new CopaymentID("801"),"HMO 巡迴醫療",true),
            new Copayment(new CopaymentID("802"),"蘭綠計畫",true),
            new Copayment(new CopaymentID("901"),"多氯聯苯中毒之油症患者",true,true),
            new Copayment(new CopaymentID("902"),"三歲以下兒童醫療補助計畫",true,true),
            new Copayment(new CopaymentID("903"),"新生兒依附註記方式就醫者",true,true),
            new Copayment(new CopaymentID("904"),"行政協助愛滋病案件、愛滋防治替代治療計畫",true,true),
            new Copayment(new CopaymentID("905"),"三氯氰胺污染奶製品案",false),
            new Copayment(new CopaymentID("906"),"內政部役政署補助替代役役男全民健康保險自行負擔醫療費用",true,true),
            new Copayment(new CopaymentID("907"),"原住民於非山地暨離島地區接受戒菸服務者",true,true),
            new Copayment(new CopaymentID("908"),"代辦海洋委員會海巡署補助部分負擔",true),
            new Copayment(new CopaymentID("909"),"代辦中央警察大學補助部分負擔",true),
            new Copayment(new CopaymentID("910"),"代辦內政部消防署補助部分負擔",true),
            new Copayment(new CopaymentID("911"),"代辦內政部空勤總隊補助部分負擔",true),
            new Copayment(new CopaymentID("912"),"代辦內政部警政署補助部分負擔",true),
            new Copayment(new CopaymentID("913"),"代辦國防部補助部分負擔",true)
        };

        internal static List<Copayment> GetCopaymentList()
        {
            return CopaymentList;
        }
        internal static Copayment GetCopaymentById(string id)
        {
            return CopaymentList.Single(_ => _.CopaymentID.ID.Equals(id));
        }
    }

    public static class CopaymentService
    {
        public static List<Copayment> GetCopaymentList()
        {
            return CopaymentRepository.GetCopaymentList();
        }

        public static bool IsExist(CopaymentID id)
        {
            return GetCopaymentList().Exists(c => c.CopaymentID.Equals(id));
        }
        /* 100元以下 0元
           101～200元 20元
           201～300元 40元      
           301～400元 60元       
           401～500元 80元          
           501～600元 100元          
           601～700元 120元          
           701～800元 140元           
           801～900元 160元
           901～1000元 180元
           1001元以上 200元
         */
        public static int CountCopaymentPoint(Copayment copayment, int medicinePoint)
        {
            if (copayment.IsFree) return 0;
            return medicinePoint switch
            {
                <= 100 => 0,
                >= 101 and <= 200 => 20,
                >= 201 and <= 300 => 40,
                >= 301 and <= 400 => 60,
                >= 401 and <= 500 => 80,
                >= 501 and <= 600 => 100,
                >= 601 and <= 700 => 120,
                >= 701 and <= 800 => 140,
                >= 801 and <= 900 => 160,
                >= 901 and <= 1000 => 180,
                _ => 200
            };
        }

        public static Copayment GetCopayment(Copayment copayment, int medicinePoint,AdjustCase adjustCase,Division division)
        {
            if (AdjustCaseService.IsOwnExpense(adjustCase))
                return CopaymentRepository.GetCopaymentById("");
            if (!string.IsNullOrEmpty(division.DivisionID.ID) && DivisionService.IsDentistry(division) || AdjustCaseService.IsChronic(adjustCase))
                return CopaymentRepository.GetCopaymentById("I22");
            return medicinePoint switch
            {
                <= 100 when string.IsNullOrEmpty(copayment.CopaymentID.ID) || 
                            copayment.CopaymentID.ID.Equals("I22") && !IsFreeProject(adjustCase,division) => CopaymentRepository.GetCopaymentById("I21"),
                > 100 when copayment.CopaymentID.ID == "I21" => CopaymentRepository.GetCopaymentById("I20"),
                <= 100 when copayment.CopaymentID.ID == "I20" => CopaymentRepository.GetCopaymentById("I21"),
                _ => copayment
            };
        }

        private static bool IsFreeProject(AdjustCase adjustCase, Division division)
        {
            return adjustCase.AdjustCaseID.ID == Resources.ChronicAdjustCaseID || DivisionService.IsDentistry(division);
        }

        public static Copayment GetCopaymentById(CopaymentID id)
        {
            return GetCopaymentList().SingleOrDefault(c => c.CopaymentID.Equals(id));
        }
    }
}