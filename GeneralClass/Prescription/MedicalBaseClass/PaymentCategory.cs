using System.Collections.Generic;
using System.Linq;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalBaseClass
{
    /*
     * 一、請依原處方所列給付類別代碼，如下列填報：
     * 1:職業傷害 2:職業病
     * 3:普通傷害 4:普通疾病
     * 8:天然災害（88.9 增訂）
     * 9:呼吸照護（89.7 增訂）
     * A:天然災害-巡迴(98.8 增訂，目前暫停使用)
     * B:天然災害-非巡迴(98.8增訂，目前暫停使用)
     * Y:八仙樂園粉塵爆燃事件（104.06.27增訂）
     * Z:高雄市氣爆事件(103.07.31增訂)
     * 二、資料格式30(藥局)：慢性病連續處方調劑(案件分類2)及藥事居家照護（案件分類D），本
     * 欄免填。
     */

    public readonly struct PaymentCategory
    {
        public PaymentCategory(PaymentCategoryID id, string name)
        {
            ID = id;
            Name = name;
        }
        public PaymentCategoryID ID { get; }
        public string Name { get; }
    }

    public static class PaymentCategoryRepository
    {
        private static readonly List<PaymentCategory> PaymentCategoryList = new()
        {
            new PaymentCategory(new PaymentCategoryID("1"),"職業傷害"),
            new PaymentCategory(new PaymentCategoryID("2"),"職業病"),
            new PaymentCategory(new PaymentCategoryID("3"),"普通傷害"),
            new PaymentCategory(new PaymentCategoryID("4"),"普通疾病"),
            new PaymentCategory(new PaymentCategoryID("8"),"天然災害"),
            new PaymentCategory(new PaymentCategoryID("9"),"呼吸照護"),
            new PaymentCategory(new PaymentCategoryID("Y"),"八仙樂園粉塵爆燃事件"),
            new PaymentCategory(new PaymentCategoryID("Z"),"高雄市氣爆事件")
        };

        internal static List<PaymentCategory> GetPaymentCategoryList()
        {
            return PaymentCategoryList;
        }
    }

    public static class PaymentCategoryService
    {
        public static List<PaymentCategory> GetPaymentCategoryList()
        {
            return PaymentCategoryRepository.GetPaymentCategoryList();
        }

        public static bool IsExist(PaymentCategoryID paymentCategoryId)
        {
            return GetPaymentCategoryList().Exists(p => p.ID.Equals(paymentCategoryId));
        }

        public static PaymentCategory GetPaymentCategoryById(PaymentCategoryID id)
        {
            return GetPaymentCategoryList().SingleOrDefault(dataPaymentCategoryID => dataPaymentCategoryID.ID.Equals(id));
        }
    }
}
