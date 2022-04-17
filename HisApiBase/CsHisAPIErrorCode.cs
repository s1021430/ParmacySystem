using System.ComponentModel;

namespace HisApiBase
{
    public enum CsHisAPIErrorCode
    {
        [Description("開啟/關閉COM埠失敗")]
        OpenCOMFailed = -1,
        [Description("成功")]
        Success = 0,
        [Description("讀卡機逾時")]
        TimeOut = 4000,
        [Description("未置入健保IC卡")]
        ICCardNotFound = 4013,
        [Description("IC卡權限不足")]
        ICPermissionDenied = 4029,
        [Description("所置入非健保IC卡")]
        InvalidCard =4033,
        [Description("安全模組尚未與IDC認證")]
        SAMNotCertifiedWithIDC = 4050,
        [Description("卡片已過有限期限")]
        CardExpired=5003,
        [Description("網路不通")]
        InternetDisconnected=4061,
        [Description("健保IC卡與IDC認證失敗")]
        ICCardNotCertifiedWithIDC=4071,
        [Description("就醫可用次數不足")]
        TreatAvailableTimesNotEnough=5001,
        [Description("卡片已註銷")]
        CardLoggedOut=5002,
        [Description("新生兒依附就醫已逾60日")]
        BabyTreatPassed60Days=5004,
        [Description("讀卡機的就診日期時間讀取失敗")]
        GetTreatDateFailed=5005,
        [Description("讀取安全模組之醫療院所代碼失敗")]
        GetInstitutionIDFailed=5006,
        [Description("寫入一組新的就醫資料登錄失敗")]
        WriteTreatDataFailed=5007,
        [Description("安全模組簽章失敗")]
        SAMSignatureFailed=5008,
        [Description("投保單位無權限")]
        InstitutionNoPermission=5009,
        [Description("同一天看診兩科(含)以上")]
        SameDateTreat=5010,
        [Description("此人未在保")]
        PatientNotInsured=5012,
        [Description("最近24小時內同院所未曾就醫，故不可取消就醫（就醫類別輸入 ZA/ZB 時檢查）")]
        TreatmentCancelledFailed=5081,
        [Description("持卡人於非所限制的醫療院所就診")]
        InstitutionIsNotDesignated=9129
    }
}
