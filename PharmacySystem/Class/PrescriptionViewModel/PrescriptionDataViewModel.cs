using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GeneralClass.Employee;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Prescription.MedicalOrders;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.ViewModel;

namespace PharmacySystem.Class.PrescriptionViewModel
{
    public class PrescriptionDataViewModel : ObservableObject
    {
        public static explicit operator PrescriptionDataViewModel(PrescriptionData data)
        {
            return new PrescriptionDataViewModel(data);
        }

        public static implicit operator PrescriptionData(PrescriptionDataViewModel viewModel)
        {
            var model = new PrescriptionData
            {
                Patient = viewModel.Patient,
                InstitutionID = new InstitutionID(viewModel.Institution.ID),
                DivisionID = viewModel.Division.DivisionID,
                AdjustDate = viewModel.AdjustDate,
                TreatDate = viewModel.TreatDate,
                MedicalSerialNum = viewModel.MedicalSerialNum,
                CopaymentID = viewModel.Copayment.CopaymentID,
                PaymentCategoryID = viewModel.PaymentCategory.ID,
                MainDiseaseID = viewModel.MainDisease.ID,
                SubDiseaseID = viewModel.SubDisease.ID,
                PrescriptionCaseID = viewModel.PrescriptionCase.PrescriptionCaseID,
                SpecialTreatID = viewModel.SpecialTreat.ID,
                CopaymentPoint = viewModel.CopaymentPoint,
                MedicalServicePoint = viewModel.MedicalServicePoint,
                SpecialMaterialPoint = viewModel.SpecialMaterialPoint,
                MedicinePoint = viewModel.MedicinePoint,
                TotalPoint = viewModel.TotalPoint,
                AdjustCaseID = viewModel.AdjustCase.AdjustCaseID,
                MedicineSelfPayAmount = viewModel.MedicineSelfPayAmount,
                Pharmacist = viewModel.Pharmacist,
                MedicalOrders = new List<MedicalOrder>(),
                MedicineDays = viewModel.MedicineDays
            };
            foreach (var orderViewModel in viewModel.MedicalOrders.Where(_ => _.Type != MedicalOrderType.Null))
            {
                MedicalOrder medicalOrder = orderViewModel.Type switch
                {
                    MedicalOrderType.Medicine => (MedicineOrder)orderViewModel,
                    MedicalOrderType.SpecialMaterial => (SpecialMaterialOrder)orderViewModel,
                    MedicalOrderType.SpecialMaterialNoSubsidy => (SpecialMaterialNoSubsidyOrder)orderViewModel,
                    MedicalOrderType.SpecialMaterialNotInPaymentRule => (SpecialMaterialNotInPaymentRuleOrder)orderViewModel,
                    MedicalOrderType.Virtual => (VirtualOrder)orderViewModel,
                    MedicalOrderType.MedicalService => (MedicalServiceOrder)orderViewModel,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                medicalOrder.ExecutionStart = model.AdjustDate.Date;
                medicalOrder.ExecutionEnd = model.AdjustDate.Date;
                model.MedicalOrders.Add(medicalOrder);
            }
            return model;
        }

        public PrescriptionDataViewModel()
        {
            
        }

        public PrescriptionDataViewModel(PrescriptionData data)
        {
            Patient = (PatientDataViewModel)data.Patient;
            Institution = InstitutionServiceProvider.Service.GetInstitutionById(data.InstitutionID.ID).FirstOrDefault();
            Division = DivisionService.GetDivisionById(data.DivisionID);
            AdjustDate = data.AdjustDate;
            TreatDate = data.TreatDate;
            MedicalSerialNum = data.MedicalSerialNum;
            Copayment = CopaymentService.GetCopaymentById(data.CopaymentID);
            var diseaseService = new DiseaseCodeApplicationService();
            MainDisease = diseaseService.GetDiseasesById(data.MainDiseaseID.ID);
            SubDisease = diseaseService.GetDiseasesById(data.SubDiseaseID.ID);
            PrescriptionCase = PrescriptionCaseService.GetPrescriptionCaseById(data.PrescriptionCaseID);
            SpecialTreat = SpecialTreatService.GetSpecialTreatByID(data.SpecialTreatID);
            CopaymentPoint = data.CopaymentPoint;
            MedicalServicePoint = data.MedicalServicePoint;
            SpecialMaterialPoint = data.SpecialMaterialPoint;
            MedicinePoint = data.MedicinePoint;
            TotalPoint = data.TotalPoint;
            AdjustCase = AdjustCaseService.GetAdjustCaseById(data.AdjustCaseID);
            MedicineSelfPayAmount = data.MedicineSelfPayAmount;
            PaymentCategory = PaymentCategoryService.GetPaymentCategoryById(data.PaymentCategoryID);
            Pharmacist = ViewModelLocator.Locator.Dispense.PharmacistList.First();
            MedicalOrders = new ObservableCollection<MedicalOrderViewModel>();
            foreach (var order in data.MedicalOrders)
            {
                MedicalOrders.Add(new MedicalOrderViewModel(order));
            }
            MedicineDays = data.MedicineDays;
        }
        private PatientDataViewModel patient = new();
        public PatientDataViewModel Patient
        {
            get => patient;
            set
            {
                Set(() => Patient, ref patient, value);
            }
        }
        private Institution institution = new();
        public Institution Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
            }
        }

        private Division division;
        public Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
            }
        }

        private AdjustCase adjustCase;
        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                Set(() => AdjustCase, ref adjustCase, value);
            }
        }

        private Employee pharmacist;
        public Employee Pharmacist
        {
            get => pharmacist;
            set
            {
                Set(() => Pharmacist, ref pharmacist, value);
            }
        }

        private string medicalSerialNum;
        public string MedicalSerialNum
        {
            get => medicalSerialNum;
            set
            {
                Set(() => MedicalSerialNum, ref medicalSerialNum, value);
            }
        }

        private Copayment copayment;
        public Copayment Copayment
        {
            get => copayment;
            set
            {
                Set(() => Copayment, ref copayment, value);
            }
        }

        private PrescriptionCase prescriptionCase;
        public PrescriptionCase PrescriptionCase
        {
            get => prescriptionCase;
            set
            {
                Set(() => PrescriptionCase, ref prescriptionCase, value);
            }
        }

        private DateTime treatDate = DateTime.Now;
        public DateTime TreatDate
        {
            get => treatDate;
            set
            {
                Set(() => TreatDate, ref treatDate, value);
            }
        }

        private DateTime adjustDate = DateTime.Now;
        public DateTime AdjustDate
        {
            get => adjustDate;
            set
            {
                Set(() => AdjustDate, ref adjustDate, value);
            }
        }

        private DiseaseCode mainDisease = new(new DiseaseCodeID(string.Empty), string.Empty);
        public DiseaseCode MainDisease
        {
            get => mainDisease;
            set
            {
                Set(() => MainDisease, ref mainDisease, value);
            }
        }

        private DiseaseCode subDisease = new(new DiseaseCodeID(string.Empty), string.Empty);
        public DiseaseCode SubDisease
        {
            get => subDisease;
            set
            {
                Set(() => SubDisease, ref subDisease, value);
            }
        }

        private PaymentCategory paymentCategory;
        public PaymentCategory PaymentCategory
        {
            get => paymentCategory;
            set
            {
                Set(() => PaymentCategory, ref paymentCategory, value);
            }
        }

        private SpecialTreat specialTreat;
        public SpecialTreat SpecialTreat
        {
            get => specialTreat;
            set
            {
                Set(() => SpecialTreat, ref specialTreat, value);
            }
        }

        private ObservableCollection<MedicalOrderViewModel> medicalOrders = new() { new MedicalOrderViewModel()};
        public ObservableCollection<MedicalOrderViewModel> MedicalOrders
        {
            get => medicalOrders;
            set
            {
                Set(() => MedicalOrders, ref medicalOrders, value);
            }
        }

        private int medicinePoint;
        public int MedicinePoint
        {
            get => medicinePoint;
            set
            {
                Set(() => MedicinePoint, ref medicinePoint, value);
            }
        }

        private int specialMaterialPoint;
        public int SpecialMaterialPoint
        {
            get => specialMaterialPoint;
            set
            {
                Set(() => SpecialMaterialPoint, ref specialMaterialPoint, value);
            }
        }

        private int copaymentPoint;
        public int CopaymentPoint
        {
            get => copaymentPoint;
            set
            {
                Set(() => CopaymentPoint, ref copaymentPoint, value);
            }
        }

        private int medicalServicePoint;
        public int MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
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

        private int medicineSelfPayAmount;
        public int MedicineSelfPayAmount
        {
            get => medicineSelfPayAmount;
            set
            {
                if (Set(() => MedicineSelfPayAmount, ref medicineSelfPayAmount, value))
                    CountAmountReceivable();
            }
        }

        private double amountReceivable;
        public double AmountReceivable
        {
            get => amountReceivable;
            set
            {
                Set(() => AmountReceivable, ref amountReceivable, value);
            }
        }

        private double paymentAmount;
        public double PaymentAmount
        {
            get => paymentAmount;
            set
            {
                if (Set(() => PaymentAmount, ref paymentAmount, value))
                    CountChange();
            }
        }

        private void CountChange()
        {
            var tempChange = paymentAmount - amountReceivable;
            Change = tempChange > 0 ? tempChange : 0;
        }

        private double change;
        public double Change
        {
            get => change;
            private set
            {
                Set(() => Change, ref change, value);
            }
        }
        public int MedicineDays { get; set; }

        private int chronicCurrentTimes;
        public int ChronicCurrentTimes
        {
            get => chronicCurrentTimes;
            set
            {
                Set(() => ChronicCurrentTimes, ref chronicCurrentTimes, value);
            }
        }

        private int chronicAvailableTimes;
        public int ChronicAvailableTimes
        {
            get => chronicAvailableTimes;
            set
            {
                Set(() => ChronicAvailableTimes, ref chronicAvailableTimes, value);
            }
        }
        public void CountAmountReceivable()
        {
            AmountReceivable = CopaymentPoint + MedicineSelfPayAmount;
        }

        internal void CountPoint()
        {
            MedicinePoint = MedicalOrders.Where(order => order.Type == MedicalOrderType.Medicine && !order.OwnExpense).Sum(order => order.TotalPoint);
            MedicineSelfPayAmount = MedicalOrders.Where(order => order.OwnExpense).Sum(order => order.TotalPoint);
            CheckCopayment();
            TotalPoint = MedicinePoint + CopaymentPoint;
        }

        public void CheckCopayment()
        {
            Copayment = CopaymentService.GetCopayment(Copayment, MedicinePoint, AdjustCase, Division);
            CopaymentPoint = CopaymentService.CountCopaymentPoint(Copayment, MedicinePoint);
        }

        public void CountMedicineDays()
        {
            var orderWithMedicinesDays = MedicalOrders.Where(_ => _.Type is MedicalOrderType.Medicine && _.Days != null).ToArray();
            if(!orderWithMedicinesDays.Any()) return;
            MedicineDays = orderWithMedicinesDays.Max(_ => (int)_.Days);
        }
    }
}
