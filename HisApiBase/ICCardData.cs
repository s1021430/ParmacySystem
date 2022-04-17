using System;
using System.Collections.Generic;
using GeneralClass.Customer;

namespace HisApiBase
{
    public class IcCardData
    {
        public readonly string CardNumber;
        public readonly string Name; 
        public readonly string IDNumber; 
        public readonly DateTime Birthday;  //民國年
        public readonly Gender Gender; 
        public readonly DateTime CardReleaseDate;  //民國年
        public readonly string CardLogoutMark; 
        public readonly string Tel;
        public readonly bool IsSuccess = false;
        /*
         * 卡片號碼(1-12)
         * 姓名(13-32)
         * 身分證號或身分證明文件號碼(33-42)
         * 出生日期(43-49) 
         * 性別(50)
         * 發卡日期(51-57)
         * 卡片註銷註記(58)
         * 連絡電話(59-72)
         */
        public IcCardData(byte[] pBuffer)
        {
            CardNumber = ConvertData.BytesToString(pBuffer,0,12);
            Name = ConvertData.BytesToString(pBuffer,12,20).Trim();
            IDNumber = ConvertData.BytesToString(pBuffer,32,10);
            Birthday = ConvertData.TaiwanCalendarStringToDateTime(ConvertData.BytesToString(pBuffer, 42, 7)); 
            Gender = ConvertData.BytesToString(pBuffer,49,1).Equals("M") ? Gender.男 : Gender.女;
            CardReleaseDate = ConvertData.TaiwanCalendarStringToDateTime(ConvertData.BytesToString(pBuffer, 50, 7)); 
            CardLogoutMark = ConvertData.BytesToString(pBuffer,57,1);
            Tel = ConvertData.BytesToString(pBuffer,58,14);
            IsSuccess = true;
        }

        public IcCardData(string cardNumber, string name, string idNumber, DateTime birthday, Gender gender,
            DateTime cardReleaseDate, string logoutMark, string tel)
        {
            CardNumber = cardNumber;
            Name = name;
            IDNumber = idNumber;
            Birthday = birthday;
            Gender = gender;
            CardReleaseDate = cardReleaseDate;
            CardLogoutMark = logoutMark;
            Tel = tel;
            IsSuccess = true;
        }
        public IcCardData() { }
    }

    public class SeqNumber
    {
        public DateTime TreatDateTime;
        public string MedicalNumber;
        public string InstitutionId;
        public string SecuritySignature;
        public string SamId;
        public bool SameDayTreat;
        public bool IsSuccess = false;
        public SeqNumber(byte[] pBuffer)
        {
            var treatDateTimeString = ConvertData.BytesToString(pBuffer,0,13);
            TreatDateTime = ConvertData.TaiwanCalendarStringToDateTime(treatDateTimeString,TimeUnit.Sec);
            MedicalNumber = ConvertData.BytesToString( pBuffer, 13,4).Trim();
            InstitutionId = ConvertData.BytesToString( pBuffer, 17,10);
            SecuritySignature = ConvertData.BytesToString(pBuffer, 27,256);
            SamId = ConvertData.BytesToString(pBuffer, 283,12);
            SameDayTreat = ConvertData.BytesToString(pBuffer, 295,1).Equals("Y");
            IsSuccess = true;
        }

        public SeqNumber() { }
    }

    public class HisPrescriptionData
    {
        public readonly List<OutpatientPrescription> OutpatientPrescriptions;
        public List<LongTermPrescription> LongTermPrescriptions; 
        public List<ImportantTreatment> ImportantTreatments; 
        public List<IrritationDrug> IrritationDrugs;
        public bool IsSuccess;
        /*
         * OutpatientPrescriptions 門診處方箋 60 組
         * LongTermPrescriptions 長期處方箋 30 組
         * ImportantTreatmentCodes 重要醫令 10 組
         * IrritationDrugs 過敏藥物 3 組
         */
        public HisPrescriptionData(byte[] pOutpatientPrescription,byte[] pLongTermPrescription,byte[] pImportantTreatmentCode,byte[] pIrritationDrug)
        {
            OutpatientPrescriptions = new List<OutpatientPrescription>();
            LongTermPrescriptions = new List<LongTermPrescription>();
            ImportantTreatments = new List<ImportantTreatment>();
            IrritationDrugs = new List<IrritationDrug>();
            var currentByte = 0;
            while (currentByte < 3660)
            {
                OutpatientPrescriptions.Add(new OutpatientPrescription(ConvertData.ByteGetSubArray(pOutpatientPrescription,currentByte,61)));
                currentByte += 61;
            }
            currentByte = 0;
            while (currentByte < 1320)
            {
                LongTermPrescriptions.Add(new LongTermPrescription(ConvertData.ByteGetSubArray(pLongTermPrescription,currentByte,44)));
                currentByte += 44;
            }
            currentByte = 0;
            while (currentByte < 360)
            {
                ImportantTreatments.Add(new ImportantTreatment(ConvertData.ByteGetSubArray(pImportantTreatmentCode,currentByte,36)));
                currentByte += 36;
            }
            currentByte = 0;
            while (currentByte < 120)
            {
                IrritationDrugs.Add(new IrritationDrug(ConvertData.ByteGetSubArray(pIrritationDrug,currentByte,40)));
                currentByte += 40;
            }
            IsSuccess = true;
        }

        public HisPrescriptionData() { }
    }

    public readonly struct OutpatientPrescription
    {
        public readonly DateTime TreatDateTime;
        public readonly string MedicalOrder; 
        public readonly string TreatmentItemCode; 
        public readonly string TreatmentPosition;
        public readonly string Usage; 
        public readonly double Days;
        public readonly double TotalAmount;
        public readonly string PrescriptionDeliverMark;
        /*
         門診處方箋資料
         就診日期時間(1-13)
         醫令類別(14)
         診療項目代號(15-26)
         診療部位(27-32)
         用法(33-50)
         天數(51-52)
         總量(53-59)
         交付處方註記(60-61)
        */
        public OutpatientPrescription(byte[] pBuffer)
        {
            TreatDateTime = ConvertData.TaiwanCalendarStringToDateTime(ConvertData.BytesToString(pBuffer, 0, 13),TimeUnit.Sec);
            MedicalOrder = ConvertData.BytesToString(pBuffer, 13, 1);
            TreatmentItemCode = ConvertData.BytesToString(pBuffer, 14, 12);
            TreatmentPosition = ConvertData.BytesToString(pBuffer, 26, 6);
            Usage = ConvertData.BytesToString(pBuffer, 32, 18);
            Days = 0;
            if(int.TryParse(ConvertData.BytesToString(pBuffer, 50, 2).Trim(),out var days))
                Days = days;
            TotalAmount = 0;
            if(double.TryParse(ConvertData.BytesToString(pBuffer, 52, 7).Trim(),out var total))
                TotalAmount = total;
            PrescriptionDeliverMark = ConvertData.BytesToString(pBuffer, 59, 2);
        }
    }

    public readonly struct LongTermPrescription
    {
        public readonly DateTime CreateDate;
        public readonly string MedicalID; 
        public readonly string Usage; 
        public readonly double Days;
        public readonly double TotalAmount;
        /*
        開立日期(1-7)
        藥品代碼(8-17)
        用法(18-35)
        天數(36-37)
        總量(38-44)
         */
        public LongTermPrescription(byte[] pBuffer)
        {
            CreateDate = ConvertData.TaiwanCalendarStringToDateTime(ConvertData.BytesToString(pBuffer, 0, 7));
            MedicalID = ConvertData.BytesToString(pBuffer, 7, 10);
            Usage = ConvertData.BytesToString(pBuffer, 17, 18);
            Days = 0;
            if(int.TryParse(ConvertData.BytesToString(pBuffer, 35, 2).Trim(),out var days))
                Days = days;
            TotalAmount = 0;
            if(double.TryParse(ConvertData.BytesToString(pBuffer, 37, 7).Trim(),out var total))
                TotalAmount = total;
        }
    }

    public readonly struct ImportantTreatment
    {
        public readonly DateTime ExecutionDate;
        public readonly string InstitutionID; 
        public readonly string ImportantTreatmentCode; 
        public readonly string ExecutionPositionCode;
        public readonly double TotalAmount;
        /*
        實施日期(1-7)
        醫療院所代碼(8-17)
        重要醫令項目代碼(18-23)
        實施部位代碼(24-29)
        總量(30-36)
         */
        public ImportantTreatment(byte[] pBuffer)
        {
            ExecutionDate = ConvertData.TaiwanCalendarStringToDateTime(ConvertData.BytesToString(pBuffer, 0, 7));
            InstitutionID = ConvertData.BytesToString(pBuffer, 7, 10);
            ImportantTreatmentCode = ConvertData.BytesToString(pBuffer, 17, 6);
            ExecutionPositionCode = ConvertData.BytesToString(pBuffer, 23, 6);
            TotalAmount = 0;
            if(double.TryParse(ConvertData.BytesToString(pBuffer, 29, 7).Trim(),out var total))
                TotalAmount = total;
        }
    }

    public struct IrritationDrug
    {
        public readonly string IrritationDrugName; 
        //過敏藥物成份名稱
        public IrritationDrug(byte[] pBuffer)
        {
            IrritationDrugName = ConvertData.BytesToString(pBuffer, 0, 40);
        }
    }
}
