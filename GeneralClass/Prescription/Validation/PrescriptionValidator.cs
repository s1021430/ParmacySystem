using System.Collections.Generic;
using System.ComponentModel;
using GeneralClass.Common;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Prescription.Validation.Specification;
using GeneralClass.Specification;

namespace GeneralClass.Prescription.Validation
{
    public enum PrescriptionErrorCode
    {
        [Description("病患身分證字號格式錯誤")] 
        PatientIdNumberInvalid,
        [Description("病患姓名未填寫")] 
        PatientNameEmpty,
        [Description("病患生日未填寫")] 
        PatientBirthdayNull,
        [Description("未選擇調劑案件")] 
        AdjustCaseNullOrNotExist,
        [Description("未填寫醫療院所")] 
        InstitutionNullOrNotExist,
        [Description("未選擇就醫科別")] 
        DivisionNullOrNotExist,
        [Description("未選擇原處方案件")] 
        PrescriptionCaseNullOrNotExist,
        [Description("藥品天數未填寫")] 
        MedicineDaysInvalid,
        [Description("慢箋調劑次數錯誤")] 
        ChronicAdjustTimesInvalid,
        [Description("調劑日不可早於就醫日")] 
        AdjustDateEarlierThanTreatmentDate,
        [Description("處方已過可調劑日期")] 
        AdjustDateOutOfRange,
        [Description("慢箋就醫序號格式錯誤")] 
        ChronicMedicalSerialNumberInvalid,
        [Description("就醫序號格式錯誤")]
        MedicalSerialNumberInvalid,
        [Description("未選擇藥師")] 
        PharmacistNull,
        [Description("藥師身分證字號格式錯誤")] 
        PharmacistIdNumberInvalid,
        [Description("特定治療錯誤")] 
        SpecialTreatInvalid,
        [Description("未選擇給付類別")] 
        PaymentCategoryEmpty,
        [Description("給付類別不存在")] 
        PaymentCategoryNullOrNotExist,
        [Description("主診斷代碼未填寫或疾病不存在")] 
        MainDiseaseNullOrNotExist,
        [Description("副診斷代碼未填寫或疾病不存在")] 
        SubDiseaseNotExist,
        [Description("未選擇部分負擔或部分負擔不存在")] 
        CopaymentNullOrNotExist,
        [Description("此處方需收取部分負擔，請確認部分負擔選項")] 
        ToBeChargedCopayment,
        [Description("此處方不需收取部分負擔，請確認部分負擔選項")] 
        CopaymentChargedNotNeeded,
        [Description("醫令不得空白")] 
        NoMedicalOrder,
        [Description("醫令數量不得為零")] 
        MedicalOrderAmountZero,
        [Description("藥品用量未填寫")] 
        MedicineOrderDosageEmpty,
        [Description("藥品使用頻率未填寫")] 
        MedicineOrderFrequencyEmpty,
        [Description("藥品途徑未填寫")] 
        MedicineOrderPositionEmpty,
        [Description("藥品天數未填寫")] 
        MedicineOrderDaysEmpty,
        [Description("藥品單價不得為負數")] 
        UnitPriceError,
        [Description("存取失敗")] 
        DBInsertError,
        Success,
    }

    public interface IPrescriptionValidator
    {
        PrescriptionErrorCode ValidateBeforeRegister(PrescriptionData prescription);
    }

    public class PrescriptionValidator : IPrescriptionValidator
    {
        private readonly List<PrescriptionValidateRule> specificationsBeforeRegister;

        public PrescriptionValidator(IInstitutionRepository institutionRepository,IDiseaseCodeRepository diseaseCodeRepository,IHolidayRepository holidayRepository)
        {
            specificationsBeforeRegister = new List<PrescriptionValidateRule>
            {
                new(PrescriptionErrorCode.PatientIdNumberInvalid, new PatientIdNumberSpecification()),
                new(PrescriptionErrorCode.PatientNameEmpty, new PatientNameSpecification()),
                new(PrescriptionErrorCode.PatientBirthdayNull, new PatientBirthdaySpecification()),
                new(PrescriptionErrorCode.AdjustCaseNullOrNotExist, new AdjustCaseIdSpecification()),
                new(PrescriptionErrorCode.InstitutionNullOrNotExist, new InstitutionSpecification(institutionRepository)),
                new(PrescriptionErrorCode.DivisionNullOrNotExist, new DivisionSpecification()),
                new(PrescriptionErrorCode.PrescriptionCaseNullOrNotExist, new PrescriptionCaseSpecification()),
                new(PrescriptionErrorCode.MedicineDaysInvalid, new MedicineDaysSpecification()),
                new(PrescriptionErrorCode.ChronicAdjustTimesInvalid, new ChronicAdjustTimesSpecification()),
                new(PrescriptionErrorCode.AdjustDateEarlierThanTreatmentDate, new AdjustDateLaterThanTreatmentDateSpecification()),
                new(PrescriptionErrorCode.AdjustDateOutOfRange, new TreatmentDateRangeSpecification(holidayRepository)),
                new(PrescriptionErrorCode.AdjustDateOutOfRange, new ChronicAdjustDateSpecification()),
                new(PrescriptionErrorCode.ChronicMedicalSerialNumberInvalid, new ChronicMedicalSerialNumberSpecification()),
                new(PrescriptionErrorCode.MedicalSerialNumberInvalid, new MedicalSerialNumberSpecification()),
                new(PrescriptionErrorCode.PharmacistNull, new PharmacistNotNullSpecification()),
                new(PrescriptionErrorCode.PharmacistIdNumberInvalid, new PharmacistIdNumberSpecification()),
                new(PrescriptionErrorCode.SpecialTreatInvalid, new SpecialTreatSpecification()),
                new(PrescriptionErrorCode.PaymentCategoryEmpty, new PaymentCategoryNullSpecification()),
                new(PrescriptionErrorCode.PaymentCategoryNullOrNotExist, new PaymentCategoryExistSpecification()),
                new(PrescriptionErrorCode.MainDiseaseNullOrNotExist, new MainDiseaseExistSpecification(diseaseCodeRepository)),
                new(PrescriptionErrorCode.SubDiseaseNotExist, new SubDiseaseExistSpecification(diseaseCodeRepository)),
                new(PrescriptionErrorCode.CopaymentNullOrNotExist, new CopaymentExistSpecification()),
                new(PrescriptionErrorCode.ToBeChargedCopayment, new CopaymentChargedSpecification()),
                new(PrescriptionErrorCode.CopaymentChargedNotNeeded, new CopaymentNoChargedSpecification()),
                new(PrescriptionErrorCode.NoMedicalOrder, new MedicalOrderExistSpecification()),
                new(PrescriptionErrorCode.MedicalOrderAmountZero, new MedicalOrderAmountZeroSpecification()),
                new(PrescriptionErrorCode.MedicineOrderDosageEmpty, new MedicineOrderDosageSpecification()),
                new(PrescriptionErrorCode.MedicineOrderFrequencyEmpty, new MedicineOrderFrequencySpecification()),
                new(PrescriptionErrorCode.MedicineOrderPositionEmpty, new MedicineOrderPositionSpecification()),
                new(PrescriptionErrorCode.MedicineOrderDaysEmpty, new MedicineOrderDaysSpecification()),
                new(PrescriptionErrorCode.UnitPriceError, new UnitPriceSpecification())
            };
        }

        public PrescriptionErrorCode ValidateBeforeRegister(PrescriptionData prescription)
        {
            foreach (var validateRule in specificationsBeforeRegister)
            {
                if (!validateRule.Validate(prescription))
                    return validateRule.ErrorCode;
            }

            return PrescriptionErrorCode.Success;
        }

        private readonly struct PrescriptionValidateRule
        {
            private readonly ISpecification<PrescriptionData> specification;

            public PrescriptionErrorCode ErrorCode { get; }

            public PrescriptionValidateRule(PrescriptionErrorCode errorCode,
                ISpecification<PrescriptionData> specification)
            {
                ErrorCode = errorCode;
                this.specification = specification;
            }

            public bool Validate(PrescriptionData prescriptionData)
            {
                return specification.IsSatisfiedBy(prescriptionData);
            }
        }
    }
}
