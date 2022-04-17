using System;
using GalaSoft.MvvmLight;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalOrders;

namespace PharmacySystem.Class
{
    

    public class MedicalOrderViewModel : ObservableObject
    {
        public static explicit operator MedicalOrderViewModel(MedicalOrder model)
        {
            return new(model);
        }

        public static implicit operator MedicineOrder(MedicalOrderViewModel viewModel)
        {
            return new()
            {
                ID = new MedicalOrderID(viewModel.id),
                Dosage = viewModel.dosage,
                Frequency = viewModel.frequency,
                ActionSite = viewModel.actionSite,
                Days = viewModel.days,
                TotalAmount = viewModel.totalAmount,
                OwnExpense = viewModel.ownExpense,
                TotalPoint = viewModel.totalPoint,
                Price = viewModel.NHIPrice,
                OwnExpensePrice = viewModel.OwnExpensePrice,
                ExecutionStart = DateTime.Now.Date,
                ExecutionEnd = DateTime.Now.Date,
                BatchNumber = viewModel.BatchNumber
            };
        }

        public static implicit operator SpecialMaterialOrder(MedicalOrderViewModel viewModel)
        {
            return new()
            {
                ID = new MedicalOrderID(viewModel.id),
                Dosage = viewModel.dosage,
                Frequency = viewModel.frequency,
                ActionSite = viewModel.actionSite,
                Days = viewModel.days,
                TotalAmount = viewModel.totalAmount,
                OwnExpense = viewModel.ownExpense,
                Price = viewModel.NHIPrice,
                OwnExpensePrice = viewModel.OwnExpensePrice,
                TotalPoint = viewModel.totalPoint,
                ExecutionStart = DateTime.Now.Date,
                ExecutionEnd = DateTime.Now.Date
            };
        }

        public static implicit operator SpecialMaterialNoSubsidyOrder(MedicalOrderViewModel viewModel)
        {
            return new()
            {
                ID = new MedicalOrderID(viewModel.id),
                Dosage = viewModel.dosage,
                Frequency = viewModel.frequency,
                ActionSite = viewModel.actionSite,
                Days = viewModel.days,
                TotalAmount = viewModel.totalAmount,
                Price = viewModel.NHIPrice,
                OwnExpensePrice = viewModel.OwnExpensePrice,
                OwnExpense = true,
                TotalPoint = viewModel.totalPoint,
                ExecutionStart = DateTime.Now.Date,
                ExecutionEnd = DateTime.Now.Date
            };
        }

        public static implicit operator SpecialMaterialNotInPaymentRuleOrder(MedicalOrderViewModel viewModel)
        {
            return new()
            {
                ID = new MedicalOrderID(viewModel.id),
                Dosage = viewModel.dosage,
                Frequency = viewModel.frequency,
                ActionSite = viewModel.actionSite,
                Days = viewModel.days,
                TotalAmount = viewModel.totalAmount,
                Price = viewModel.NHIPrice,
                OwnExpensePrice = viewModel.OwnExpensePrice,
                OwnExpense = true,
                TotalPoint = viewModel.totalPoint,
                ExecutionStart = DateTime.Now.Date,
                ExecutionEnd = DateTime.Now.Date
            };
        }

        public static implicit operator VirtualOrder(MedicalOrderViewModel viewModel)
        {
            return new()
            {
                ID = new MedicalOrderID(viewModel.id),
                ChineseName = viewModel.ChineseName,
                Dosage = 0,
                TotalAmount = 0,
                OwnExpense = false,
                Price = 0,
                OwnExpensePrice = 0,
                TotalPoint = 0,
                ExecutionStart = DateTime.Now.Date,
                ExecutionEnd = DateTime.Now.Date
            };
        }

        public static implicit operator MedicalServiceOrder(MedicalOrderViewModel viewModel)
        {
            return new()
            {
                ID = new MedicalOrderID(viewModel.id),
                TotalAmount = 1,
                OwnExpense = false,
                Price = viewModel.NHIPrice,
                OwnExpensePrice = viewModel.OwnExpensePrice,
                TotalPoint = viewModel.totalPoint,
                AdditionRatio = viewModel.AdditionRatio,
                ExecutionStart = DateTime.Now.Date,
                ExecutionEnd = DateTime.Now.Date
            };
        }

        public MedicalOrderViewModel()
        {
            Type = MedicalOrderType.Null;
        }

        public MedicalOrderViewModel(MedicalOrder order)
        {
            Type = order.Type;
            ID = order.ID.ID;
            ChineseName = order.ChineseName;
            EnglishName = order.EnglishName;
            Dosage = order.Dosage;
            Frequency = order.Frequency;
            ActionSite = order.ActionSite;
            Days = order.Days;
            TotalAmount = order.TotalAmount;
            OwnExpense = order.OwnExpense;
            UnitPrice = order.OwnExpense ? order.OwnExpensePrice : order.Price;
            TotalPoint = order.TotalPoint;
        }

        public MedicalOrderType Type { get; set; }

        private string id;
        public string ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }

        private string chineseName;
        public string ChineseName
        {
            get => chineseName;
            set
            {
                Set(() => ChineseName, ref chineseName, value);
            }
        }

        private string englishName;
        public string EnglishName
        {
            get => englishName;
            set
            {
                Set(() => EnglishName, ref englishName, value);
                FullName = $"{ChineseName} {EnglishName}";
            }
        }
        private string fullName;
        public string FullName
        {
            get => fullName;
            private set
            {
                Set(() => FullName, ref fullName, value);
            }
        }
        private string frequency;
        public string Frequency
        {
            get => frequency;
            set
            {
                Set(() => Frequency, ref frequency, value);
                CountTotalAmount();
            }
        }

        private float? dosage;
        public float? Dosage
        {
            get => dosage;
            set
            {
                Set(() => Dosage, ref dosage, value);
                CountTotalAmount();
            }
        }

        private string actionSite;
        public string ActionSite
        {
            get => actionSite;
            set
            {
                Set(() => ActionSite, ref actionSite, value);
            }
        }

        private int? days;
        public int? Days
        {
            get => days;
            set
            {
                Set(() => Days, ref days, value);
                CountTotalAmount();
            }
        }

        private float totalAmount;
        public float TotalAmount
        {
            get => totalAmount;
            set
            {
                Set(() => TotalAmount, ref totalAmount, value);
                CountTotalPrice();
            }
        }

        private bool ownExpense;
        public bool OwnExpense
        {
            get => ownExpense;
            set
            {
                Set(() => OwnExpense, ref ownExpense, value);
                SetUnitPrice();
            }
        }

        private void SetUnitPrice()
        {
            UnitPrice = ownExpense ? OwnExpensePrice : NHIPrice;
        }

        public float NHIPrice { get; set; }
        private float ownExpensePrice;
        public float OwnExpensePrice
        {
            get => ownExpensePrice;
            set
            {
                Set(() => OwnExpensePrice, ref ownExpensePrice, value);
            }
        }
        private float unitPrice;
        public float UnitPrice
        {
            get => unitPrice;
            set
            {
                Set(() => UnitPrice, ref unitPrice, value);
                if (ownExpense)
                    OwnExpensePrice = value;
                CountTotalPrice();
            }
        }

        private int totalPoint;
        public int TotalPoint
        {
            get => totalPoint;
            set
            {
                Set(() => TotalPoint, ref totalPoint, value);
            }
        }

        private float inventory;
        public float Inventory
        {
            get => inventory;
            set
            {
                Set(() => Inventory, ref inventory, value);
            }
        }

        private string batchNumber;
        public string BatchNumber
        {
            get => batchNumber;
            set
            {
                Set(() => BatchNumber, ref batchNumber, value);
            }
        }

        private float AdditionRatio { get; set; } = 1.00F; //P6支付成數

        private void CountTotalAmount()
        {
            if (Dosage == null || string.IsNullOrEmpty(Frequency) || Days == null) return;
            var tempFrequency = MedicalOrderService.GetFrequency(Frequency, (int)Days);
            TotalAmount = (float)Dosage * (int)Days * tempFrequency.TimesPerDay;
        }

        private void CountTotalPrice()
        {
            TotalPoint = (int)Math.Round(TotalAmount * UnitPrice, 0, MidpointRounding.AwayFromZero);
        }
    }
}
