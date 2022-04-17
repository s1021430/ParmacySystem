using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeneralClass.Common;
using GeneralClass.Person;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Prescription.MedicalOrders;
using GeneralClass.Properties;
using GeneralClass.Specification;

namespace GeneralClass.Prescription.Validation.Specification
{
    public class PatientIdNumberSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (p.AdjustCaseID.ID == Resources.OwnExpenseAdjustCaseID)
                return true;
            var idNumber = p.Patient?.IDNumber;
            return PersonService.IdNumberValidation(idNumber) || PersonService.ResidentIDValidation(idNumber);
        }
    }
    public class PatientNameSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            return p is not null && PersonService.NameValidation(p.Patient?.Name);
        }
    }
    public class PatientBirthdaySpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p?.Patient is null) return false;
            var birthday = p.Patient.Birthday;
            if (p.AdjustCaseID.ID == Resources.OwnExpenseAdjustCaseID)
                return birthday is null || birthday <= DateTime.Today;
            return birthday is not null && birthday <= DateTime.Today;
        }
    }
    public class AdjustCaseIdSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return !string.IsNullOrEmpty(p.AdjustCaseID.ID) && AdjustCaseService.IsExist(p.AdjustCaseID);
        }
    }

    public class InstitutionSpecification : CompositeSpecification<PrescriptionData>
    {
        private readonly IInstitutionRepository institutionRepository;
        public InstitutionSpecification(IInstitutionRepository repository)
        {
            institutionRepository = repository;
        }
        /// <summary>
        /// 藥事居家照護（案件分類 D）或協助辦理門診戒菸計畫(案件分類 5)，
        /// 且直接交付指示用藥或提供「戒菸個案追蹤」或「戒菸衛教暨個案管理」】者，
        /// 本欄請填“N”。
        /// </summary>
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (string.IsNullOrEmpty(p.InstitutionID.ID)) return false;
            var institutionIDRegex = new Regex("[0-9]{10}");
            return p.AdjustCaseID.ID switch
            {
                "5" => p.InstitutionID.ID.Equals("N"),
                "D" => p.InstitutionID.ID.Equals("N"),
                _ => institutionIDRegex.IsMatch(p.InstitutionID.ID) &&
                     institutionRepository.GetInstitutionByID(p.InstitutionID.ID).Count == 1
            };
        }
    }

    public class DivisionSpecification : CompositeSpecification<PrescriptionData>
    {
        /// <summary>
        /// 藥事居家照護（案件分類 D）、協助辦理門診戒菸計畫(案件分類 5)且直接交付指示用
        /// 藥或提供「戒菸個案追蹤」或「戒菸衛教暨個案管理」者，本欄免填。
        /// </summary>
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            var noNeedDivisionAdjustCases = new List<string> { "0", "D", "5" };
            if (noNeedDivisionAdjustCases.Contains(p.AdjustCaseID.ID)) return true;
            return !string.IsNullOrEmpty(p.DivisionID.ID) && DivisionService.IsExist(p.DivisionID.ID);
        }
    }

    public class PrescriptionCaseSpecification : CompositeSpecification<PrescriptionData>
    {
        /// <summary>
        /// 藥事居家照護（案件分類 D）、協助辦理門診戒菸計畫(案件分類 5)且直接交付指示用
        /// 藥、提供「戒菸個案追蹤」或「戒菸衛教暨個案管理」】者，本欄免填。
        /// </summary>
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            var noNeedPrescriptionCaseAdjustCases = new List<string> { "0", "D", "5" };
            if (noNeedPrescriptionCaseAdjustCases.Contains(p.AdjustCaseID.ID)) return true;
            var dentalDivisionsId = new List<string> { "40", "41", "42", "43", "44", "45", "46" };
            if (dentalDivisionsId.Contains(p.DivisionID.ID) && p.AdjustCaseID.ID != "2")
                return p.PrescriptionCaseID.ID == "19";
            return PrescriptionCaseService.IsExist(p.PrescriptionCaseID);
        }
    }

    public class MedicineDaysSpecification : CompositeSpecification<PrescriptionData>
    {
        // AdjustCaseId : "0" : 自費調劑 . "D" : 藥事居家照護 . "5" : 戒菸門診
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.AdjustCaseID.ID switch
            {
                "0" => true,
                "D" => p.MedicineDays == 0,
                "5" => p.MedicineDays == 0,
                _ => p.MedicineDays > 0
            };
        }
    }

    public class ChronicAdjustTimesSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (!p.AdjustCaseID.ID.Equals(Resources.ChronicAdjustCaseID)) 
                return p.ChronicAvailableTimes == 0 && p.ChronicCurrentTimes == 0;
            if (p.ChronicAvailableTimes < 0 || p.ChronicCurrentTimes < 0) return false;
            return p.ChronicAvailableTimes >= p.ChronicCurrentTimes;
        }
    }

    public class TreatmentDateRangeSpecification : CompositeSpecification<PrescriptionData>
    {
        private IHolidayRepository holidayRepository;

        public TreatmentDateRangeSpecification(IHolidayRepository repository)
        {
            holidayRepository = repository;
        }
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (!p.AdjustCaseID.ID.Equals(Resources.NormalAdjustCaseID) && 
                !p.AdjustCaseID.ID.Equals(Resources.DispenseByDayAdjustCaseID)) return true;
            var timeSpanInDays = (p.AdjustDate.Date - p.TreatDate.Date).Days;
            var holidayCount = holidayRepository.GetHolidayCountBetweenDays(p.TreatDate, p.AdjustDate);
            return timeSpanInDays - holidayCount <= 3;
        }
    }

    public class AdjustDateLaterThanTreatmentDateSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.AdjustDate >= p.TreatDate;
        }
    }

    public class ChronicAdjustDateSpecification : CompositeSpecification<PrescriptionData>
    {
        /// <summary>
        /// 慢箋第二.三次領藥日為前次服藥完畢算起的前10日。
        /// H8:西醫-持慢性病連續處方箋領藥，預定出國，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 文字修訂）。
        /// HA:西醫-持慢性病連續處方箋領藥，返回離島地區，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 新增）。
        /// HB:西醫-持慢性病連續處方箋領藥，已出海為遠洋漁船作業船員，提供切結文件，一次領取 2 個月或 3 個月用藥量案件
        /// HC:西醫-持慢性病連續處方箋領藥，已出海為國際航線船舶作業船員，提供切結文件，一次領取 2 個月或 3 個月用藥量案件
        /// HD:西醫-持慢性病連續處方箋領藥，罕見疾病病人，提供切結文件，一次領取 2 個月或 3 個月用藥量案件（101.11 新增）。
        /// HI:西醫-經保險人認定確有一次領取該處方箋總用藥量必要之特殊病人(107.04.27 新增)
        /// </summary>
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (!p.AdjustCaseID.ID.Equals(Resources.ChronicAdjustCaseID)) return true;
            var timeSpanInDays = (p.AdjustDate.Date - p.TreatDate.Date).Days;
            var specialTreatsPreGetMedicine = new List<string> { "H8", "HA", "HB", "HC", "HD", "HI" };
            if (string.IsNullOrEmpty(p.SpecialTreatID.ID) && specialTreatsPreGetMedicine.Contains(p.SpecialTreatID.ID))
                return timeSpanInDays <= p.MedicineDays * p.ChronicAvailableTimes;
            if (p.ChronicCurrentTimes <= 1) return true;
            var medicineDays = p.MedicineDays * p.ChronicCurrentTimes;
            return timeSpanInDays < medicineDays && timeSpanInDays >= (medicineDays - 10);
        }
    }

    public class ChronicMedicalSerialNumberSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (!p.AdjustCaseID.ID.Equals(Resources.ChronicAdjustCaseID)) return true;
            var regex = new Regex("IC0[234]");
            return regex.IsMatch(p.ChronicMedicalSerialNumber);
        }
    }

    public class MedicalSerialNumberSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (p.AdjustCaseID.ID.Equals(Resources.OwnExpenseAdjustCaseID)) return true;
            if (p.AdjustCaseID.ID.Equals(Resources.MedicalHomeCareAdjustCaseID)) return p.MedicalSerialNum.Equals("N");
            return !string.IsNullOrEmpty(p.MedicalSerialNum) && p.MedicalSerialNum.Length == 4;
        }
    }

    public class PharmacistNotNullSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            return p?.Pharmacist != null;
        }
    }

    public class PharmacistIdNumberSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p?.Pharmacist is null) return false;
            var idNumber = p.Pharmacist.IDNumber;
            return PersonService.IdNumberValidation(idNumber) || PersonService.ResidentIDValidation(idNumber);
        }
    }

    public class SpecialTreatSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return string.IsNullOrEmpty(p.SpecialTreatID.ID) || SpecialTreatService.IsSpecialTreatExist(p.SpecialTreatID.ID);
        }
    }

    public class PaymentCategoryNullSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (!p.AdjustCaseID.ID.Equals(Resources.OwnExpenseAdjustCaseID) && !p.AdjustCaseID.ID.Equals(Resources.MedicalHomeCareAdjustCaseID)) return true;
            return string.IsNullOrEmpty(p.PaymentCategoryID.ID) || string.IsNullOrEmpty(p.PaymentCategoryID.ID);
        }
    }

    public class PaymentCategoryExistSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return string.IsNullOrEmpty(p.PaymentCategoryID.ID) || PaymentCategoryService.IsExist(p.PaymentCategoryID);
        }
    }

    public class MainDiseaseExistSpecification : CompositeSpecification<PrescriptionData>
    {
        private IDiseaseCodeRepository DiseaseCodeRepository;
        public MainDiseaseExistSpecification(IDiseaseCodeRepository repository)
        {
            DiseaseCodeRepository = repository;
        }
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            return !string.IsNullOrEmpty(p.MainDiseaseID.ID) && !string.IsNullOrEmpty(DiseaseCodeRepository.GetDiseasesById(p.MainDiseaseID.ID).ID.ID);
        }
    }

    public class SubDiseaseExistSpecification : CompositeSpecification<PrescriptionData>
    {
        private IDiseaseCodeRepository DiseaseCodeRepository;
        public SubDiseaseExistSpecification(IDiseaseCodeRepository repository)
        {
            DiseaseCodeRepository = repository;
        }
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            return string.IsNullOrEmpty(p.SubDiseaseID.ID) || !string.IsNullOrEmpty(DiseaseCodeRepository.GetDiseasesById(p.MainDiseaseID.ID).ID.ID);
        }
    }

    public class CopaymentExistSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return !string.IsNullOrEmpty(p.CopaymentID.ID) && CopaymentService.IsExist(p.CopaymentID);
        }
    }

    public class CopaymentChargedSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (p.CopaymentID.ID == Resources.NoChargedCopaymentID)
            {
                return p.MedicalOrders.Where(m => m is MedicineOrder && !m.OwnExpense)
                    .Sum(m => m.TotalPoint) <= 100;
            }
            return true;
        }
    }

    public class CopaymentNoChargedSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            if (p.CopaymentID.ID == Resources.ChargedCopaymentID)
            {
                return p.MedicalOrders.Where(m => m is MedicineOrder && !m.OwnExpense)
                    .Sum(m => m.TotalPoint) > 100;
            }
            return true;
        }
    }

    public class MedicalOrderExistSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.MedicalOrders.OfType<MedicineOrder>().Any() || p.MedicalOrders.OfType<SpecialMaterialOrder>().Any();
        }
    }

    public class MedicalOrderAmountZeroSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.MedicalOrders.Where(order => order is MedicineOrder or SpecialMaterialOrder)
                .All(order => order.TotalAmount > 0);
        }
    }

    public class MedicineOrderDosageSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.MedicalOrders.OfType<MedicineOrder>()
                .Where(_ => !_.OwnExpense).All(order => order.Dosage is > 0);
        }
    }

    public class MedicineOrderFrequencySpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.MedicalOrders.OfType<MedicineOrder>()
                .Where(_ => !_.OwnExpense).All(order => !string.IsNullOrEmpty(order.Frequency));
        }
    }

    public class MedicineOrderPositionSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.MedicalOrders.OfType<MedicineOrder>()
                .Where(_ => !_.OwnExpense).All(order => !string.IsNullOrEmpty(order.ActionSite));
        }
    }

    public class MedicineOrderDaysSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            return p.MedicalOrders.OfType<MedicineOrder>()
                .Where(_ => !_.OwnExpense).All(order => order.Days is > 0);
        }
    }

    public class UnitPriceSpecification : CompositeSpecification<PrescriptionData>
    {
        public override bool IsSatisfiedBy(PrescriptionData p)
        {
            if (p is null) return false;
            var declaredOrderSpecification = p.MedicalOrders.All(order => order.Price >= 0);
            var ownExpenseSpecification = p.MedicalOrders.All(order => order.OwnExpensePrice >= 0);
            return declaredOrderSpecification && ownExpenseSpecification;
        }
    }
}
