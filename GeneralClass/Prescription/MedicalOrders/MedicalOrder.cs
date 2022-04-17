using System;
using System.Diagnostics;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalOrders
{
    public enum MedicalOrderType
    {
        Null,
        Medicine,
        NoCharged,
        SpecialMaterial,
        SpecialMaterialNoSubsidy,
        SpecialMaterialNotInPaymentRule,
        Virtual,
        MedicalService
    }

    public class MedicalOrder
    {
        public MedicalOrder()
        {
        }
        public MedicalOrder(MedicalOrderID id)
        {
            ID = id;
        }
        public PrescriptionID PreMasID { get; set; }
        public MedicalOrderType Type { get; protected set; } //P1 醫令類別
        public MedicalOrderID ID { get; set; }//P2 藥品(項目)代號
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string FullName => $"{ChineseName} {EnglishName}";
        public float? Dosage { get; set; }
        public string Frequency { get; set; }
        public string ActionSite { get; set; }
        public int? Days { get; set; }
        public float TotalAmount { get; set; }
        public bool OwnExpense { get; set; }
        public float OwnExpensePrice { get; set; }//自費價
        public float Price { get; set; }//健保價
        public int TotalPoint { get; set; }
        public float? AdditionRatio { get; set; } = 1.00F; //P6支付成數
        public int MedicalOrderSerialNumber { get; set; } //P10醫令序
        public DateTime ExecutionStart { get; set; } //P12執行時間(起)
        public DateTime ExecutionEnd { get; set; } //P13執行時間(迄)
        public double Inventory { get; set; }
        public virtual void CountTotalPoint(){}
    }

    public class MedicineOrder : MedicalOrder
    {
        public string BatchNumber { get; set; }
        public MedicineOrder()
        {
            Type = MedicalOrderType.Medicine;
        }
        public override void CountTotalPoint()
        {
            TotalPoint = (int)Math.Round(TotalAmount * Price,0,MidpointRounding.AwayFromZero);
        }
    }

    public class SpecialMaterialOrder : MedicalOrder
    {
        public int OwnExpenseSerialNumber { get; set; } //P15自費特材群組序號
        public SpecialMaterialOrder()
        {
            Type = MedicalOrderType.SpecialMaterial;
            AdditionRatio = 1.05F;
        }

        public override void CountTotalPoint()
        {
            Debug.Assert(AdditionRatio != null, nameof(AdditionRatio) + " != null");
            TotalPoint = (int)Math.Round(TotalAmount * Price * (float)AdditionRatio, 0, MidpointRounding.AwayFromZero);
        }
    }

    public class SpecialMaterialNoSubsidyOrder : SpecialMaterialOrder//健保未支付自費特材項目(不可算點數)
    {
        public SpecialMaterialNoSubsidyOrder()
        {
            Type = MedicalOrderType.SpecialMaterialNoSubsidy;
            OwnExpense = true;
        }
    }

    public class SpecialMaterialNotInPaymentRuleOrder : SpecialMaterialOrder//不符給付規定自費特材項目(不可算點數)
    {
        public SpecialMaterialNotInPaymentRuleOrder()
        {
            Type = MedicalOrderType.SpecialMaterialNotInPaymentRule;
            OwnExpense = true;
        }
    }

    public class VirtualOrder : MedicalOrder
    {
        public VirtualOrder(string id,string name)
        {
            ID = new MedicalOrderID(id);
            ChineseName = name;
            EnglishName = string.Empty;
            Type = MedicalOrderType.Virtual;
            Dosage = 0;
            Frequency = string.Empty;
            ActionSite = string.Empty;
            Days = 0;
            TotalAmount = 0;
            AdditionRatio = null;
        }

        public VirtualOrder()
        {
            EnglishName = string.Empty;
            Type = MedicalOrderType.Virtual;
            Dosage = 0;
            Frequency = string.Empty;
            ActionSite = string.Empty;
            Days = 0;
            TotalAmount = 0;
            AdditionRatio = null;
        }

        public override void CountTotalPoint()
        {
            TotalPoint = 0;
        }
    }

    public class MedicalServiceOrder : MedicalOrder
    {
        public MedicalServiceOrder()
        {
            Type = MedicalOrderType.MedicalService;
            Dosage = 0;
            Frequency = string.Empty;
            ActionSite = string.Empty;
            Days = null;
            TotalAmount = 1;
            AdditionRatio = 1.00F;
        }
        public MedicalServiceOrder(int medicineDays,DateTime executionDate)
        {
            Type = MedicalOrderType.MedicalService;
            Dosage = 0;
            Frequency = string.Empty;
            ActionSite = string.Empty;
            Days = null;
            TotalAmount = 1;
            AdditionRatio = 1.00F;
            ID = medicineDays switch
            {
                < 7 => new MedicalOrderID("05202B"),
                >= 7 and <= 13 => new MedicalOrderID("05223B"),
                >= 14 and <= 27 => new MedicalOrderID("05206B"),
                >= 28 => new MedicalOrderID("05234D")
            };
            ExecutionStart = executionDate.Date;
            ExecutionEnd = executionDate.Date;
        }

        public override void CountTotalPoint()
        {
            TotalPoint = ID.ID switch
            {
                "05202B" or "05223B" => 54,
                "05206B" => 65,
                _ => 75,
            };
        }
    }
}
