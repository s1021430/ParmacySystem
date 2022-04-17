using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalOrders
{
    public class MedicalOrderService
    {
        private readonly IMedicalOrderRepository repository;

        public MedicalOrderService(IMedicalOrderRepository repository)
        {
            this.repository = repository;
        }

        public List<MedicalOrder> GetMedicalOrdersByID(List<MedicalOrderID> ordersID)
        {
            return repository.GetMedicalOrdersByID(ordersID);
        }
        /*
           QID.QIDPC		每天3餐(飯後)及(睡前)1次 4
           BID.BIDPC		每天早,晚(飯後)1次 2
           BIDH.BIDPC&HS 	每天早,晚(飯後)及睡前1次 3
           QDA.QDAC 		每天1次(早/午/晚)飯前 			1
           BIAH.BIDAC&HS 	每天早,晚(飯前)及睡前1次 		3
           QIA.TIDAC&HS 	每天三餐(飯前)及(睡前)一次  	4
           TIDP.TIDPCPRN 	需要時三餐(飯後)一次 			3
           Q8P.Q8HPRN 		需要時每八小時一次 				3
           BIDP.BIDPCPRN 	需要時早,晚(飯後)一次 			2
           Q6P.Q6HPRN 		需要時每六小時一次 				4
           HSP.QDHSPRN 	需要時睡前一次 					1
           QIDP.QIDPCPRN 	需要時三餐(飯後)及(睡前)一次 	4
           QDP.QDPCPRN 	需要時早上(飯後)一次 			1
           Q4P.Q4HPRN 		需要時每四小時一次 				6
           Q12P.Q12HPRN 	需要時每十二小時一次 			2
           Q2W 			每2星期1次 						1
           BIW 			每星期2次(每週___.___) 			2
           QW.Q1W.QWMTX	每星期1次(每週___) 				1
           TIW 			每星期3次(每週___.___.___) 		3
           Q3W 			每三星期一次(週___) 			1
         */
        public static MedicalOrderFrequency GetFrequency(string frequency,int medicineDays)
        {
            switch (frequency)
            {
                case "QD":
                    return new MedicalOrderFrequency(frequency, "每天早上(飯後)1次", 1);
                case "BL":
                    return new MedicalOrderFrequency(frequency, "中午午餐前使用", 1);
                case "BBF":
                case "QDAMAC30M":
                    return new MedicalOrderFrequency(frequency, "早餐前30分鐘", 1);
                case "BBF/BD":
                case "BIDAC30M":
                    return new MedicalOrderFrequency(frequency, "早餐前及晚餐前30分鐘", 2);
                case "BD":
                case "QDPMAC30M":
                    return new MedicalOrderFrequency(frequency, "晚餐前30分鐘", 1);
                case "QDHS":
                case "HS":
                    return new MedicalOrderFrequency(frequency, "每天睡前1次", 1);
                case "QDPM":
                    return new MedicalOrderFrequency(frequency, "每天中午(飯後)1次", 1);
                case "QDN":
                case "QN":
                    return new MedicalOrderFrequency(frequency, "每天晚上(飯後)1次", 1);
                case "QDP":
                case "QDPCPRN":
                    return new MedicalOrderFrequency(frequency, "需要時早上(飯後)一次", 1);
                case "QOD":
                    return new MedicalOrderFrequency(frequency, "每隔1天1次 ", 1);
                case "QDA":
                case "QDAC":
                    return new MedicalOrderFrequency(frequency, "每天1次(早/午/晚)飯前", 1);
                case "BIA":
                case "BIDAC":
                    return new MedicalOrderFrequency(frequency, "每天2次(早/午/晚)飯前", 2);
                case "BID":
                case "BIDPC":
                    return new MedicalOrderFrequency(frequency, "每天2次(早/午/晚)飯後", 2);
                case "BIAH":
                case "BIDAC&HS":
                    return new MedicalOrderFrequency(frequency, "每天早,晚(飯前)及睡前1次", 3);
                case "TIA":
                case "TIDAC":
                    return new MedicalOrderFrequency(frequency, "每天3次(早/午/晚)飯前", 3);
                case "TID":
                case "TIDPC":
                    return new MedicalOrderFrequency(frequency, "每天3次(早/午/晚)飯後", 3);
                case "QIA":
                case "QIDAC":
                    return new MedicalOrderFrequency(frequency, "每天3餐(飯前)及(睡前)1次", 4);
                case "QID":
                case "QIDPC":
                    return new MedicalOrderFrequency(frequency, "每天3餐(飯後)及(睡前)1次", 4);
                case "ASOR":
                case "ORDER":
                case "ASORDER":
                    return new MedicalOrderFrequency(frequency, "遵照醫師指示使用", 1);
                case "PRN":
                    return new MedicalOrderFrequency(frequency, "需要時用", 1);
                case "ST":
                case "STAT":
                    return new MedicalOrderFrequency(frequency, "即刻用", 0);
                case "QM":
                    return new MedicalOrderFrequency(frequency, "每月一次", 0);
            }
            var regexQxH = new Regex(@"Q[\d]H{1}");
            if (regexQxH.IsMatch(frequency))
            {
                var duration = int.Parse(regexQxH.Match(frequency).Groups[0].Value);
                var timesPerDay = 24 / duration;
                return new MedicalOrderFrequency(frequency,$"每{duration}小時一次",timesPerDay);
            }
            var regexQxD = new Regex(@"Q[\d]D{1}");
            if (regexQxD.IsMatch(frequency) && !frequency.Contains("&"))
            {
                var duration = int.Parse(regexQxH.Match(frequency).Groups[0].Value);
                var timesPerDay = medicineDays / duration;
                return new MedicalOrderFrequency(frequency, $"每{duration}天一次", timesPerDay);
            }
            return new MedicalOrderFrequency(frequency, $"", 1);
        }

        internal static string GetTypeId(MedicalOrderType type)
        {
            return type switch
            {
                MedicalOrderType.Medicine => "1",
                MedicalOrderType.SpecialMaterial => "3",
                MedicalOrderType.NoCharged => "4",
                MedicalOrderType.MedicalService => "9",
                MedicalOrderType.SpecialMaterialNoSubsidy => "E",
                MedicalOrderType.SpecialMaterialNotInPaymentRule => "F",
                MedicalOrderType.Virtual => "G",
                _ => "1",
            };
        }
        /*
            AD：右耳
            AS：左耳
            AU：每耳
            ET：氣切內
            GAR：漱口用
            IC：皮內注射
            IA：動脈注射
            IM：肌肉注射
            IV：靜脈注射
            IP：腹腔注射
            ICV：腦室注射
            IMP：植入
            INHL：吸入
            IS：關節腔內注射
            IT：椎骨內注射
            IVA：動脈添加
            IVD：靜脈添加
            IVP：靜脈注入
            LA：局部麻醉
            LI：局部注射
            NA=>鼻用
            OD=>右眼
            OS=>左眼
            OU=>每眼
            PO=>口服
            SC=>皮下注射
            SCI：結膜下注射
            SKIN：皮膚用
            SL：舌下
            SPI：脊髓
            RECT：肛門用
            TOPI：局部塗擦
            TPN：全靜脈營養劑
            VAG：陰道用
            IRRI：沖洗
            EXT：外用
            XX：其他
        */
        public static string GetActionSiteDescription(string orderActionSite)
        {
            var description = orderActionSite switch
            {
                "AD" => "右耳",
                "AS" => "左耳",
                "AU" => "每耳",
                "ET" => "氣切內",
                "GAR" => "漱口用",
                "IC" => "皮內注射",
                "IA" => "動脈注射",
                "IM" => "肌肉注射",
                "IV" => "靜脈注射",
                "IP" => "腹腔注射",
                "ICV" => "腦室注射",
                "IMP" => "植入",
                "INHL" => "吸入",
                "IS" => "關節腔內注射",
                "IT" => "椎骨內注射",
                "IVA" => "動脈添加",
                "IVD" => "靜脈添加",
                "IVP" => "靜脈注入",
                "LA" => "局部麻醉",
                "LI" => "局部注射",
                "NA" => "鼻用",
                "OD" => "右眼",
                "OS" => "左眼",
                "OU" => "每眼",
                "PO" => "口服",
                "SC" => "皮下注射",
                "SCI" => "結膜下注射",
                "SKIN" => "皮膚用",
                "SL" => "舌下",
                "SPI" => "脊髓",
                "RECT" => "肛門用",
                "TOPI" => "局部塗擦",
                "TPN" => "全靜脈營養劑",
                "VAG" => "陰道用",
                "IRRI" => "沖洗",
                "EXT" => "外用",
                "XX" => "其他",
                _ => string.Empty
            };
            return description;
        }
    }
}
