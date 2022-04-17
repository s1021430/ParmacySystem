using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GeneralClass;
using GeneralClass.Customer;
using GeneralClass.Prescription;

namespace HisApiBase
{
    public interface IHISAPI
    {
        IcCardData GetBasicData(); 
        SeqNumber GetSeqNumber(byte bTreatAfterCheck, string babyTreat = " ");
        bool UpdateHcContents();
        HisPrescriptionData ReadPrescriptions();
        List<string> WritePrescriptionData(PrescriptionData p);
        bool VerifySamDc();
        bool VerifyHpcPin();
        bool DailyUpload(string uploadFilePath, string recCount);
    } 
    public class HisAPI : IHISAPI
    {
        public static CsHisAPIErrorCode ErrorCode = CsHisAPIErrorCode.Success;
        /// <summary>
        /// 取得基本資料
        /// </summary>
        /// <returns></returns>
        public IcCardData GetBasicData()
        {
            
            var strLength = 72;
            var pBuffer = new byte[72];
            if (OpenCom())
            {
                var res = HisApiBase.hisGetBasicData(pBuffer, ref strLength);
                ParseErrorCode(res);
                CloseCom();
                switch (res)
                {
                    case 0: //無任何錯誤
                        return new IcCardData(pBuffer);
                    case 4000: //讀卡機 timeout
                    case 4013: //未置入健保 IC 卡
                    case 4029: //IC 卡權限不足
                    case 4033: //所置入非健保 IC 卡
                    case 4050: //安全模組尚未與 IDC 認證
                        break;
                }
            }
            return new IcCardData();
        }

        /// <summary>
        /// 取得就醫序號
        /// </summary>
        /// <returns></returns>
        public SeqNumber GetSeqNumber(byte bTreatAfterCheck, string babyTreat = " ")
        {
            var cTreatItem = ConvertData.StringToBytes("AF\0"); //就醫類別長度3個char;
            var cBabyTreat = ConvertData.StringToBytes(babyTreat); //新生兒就醫註記,長度2個char
            byte[] treatAfterCheck = {bTreatAfterCheck}; //補卡註記
            var iBufferLen = 296;
            var pBuffer = new byte[296];
            var seqNumberData = new SeqNumber();
            if (OpenCom())
            {
                var res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, treatAfterCheck, pBuffer,
                    ref iBufferLen);
                ParseErrorCode(res);
                CloseCom();
                switch (res)
                {
                    case 0:
                        seqNumberData = new SeqNumber(pBuffer);
                        break;
                    case 5003: //卡片已過有限期限
                        var updateResult = UpdateHcContents();
                        if (updateResult) //更新卡片成功，重新取就醫序號
                            seqNumberData = GetSeqNumber(bTreatAfterCheck, babyTreat);
                        break;
                    case 4000: //讀卡機 timeout
                    case 4013: //未置入健保IC卡
                    case 4029: //IC卡權限不足
                    case 4033: //所置入非健保IC卡
                    case 4050: //安全模組尚未與IDC認證
                    case 4061: //網路不通
                    case 4071: //健保IC卡與IDC認證失敗
                    case 5001: //就醫可用次數不足
                    case 5002: //卡片已註銷
                    case 5004: //新生兒依附就醫已逾60日
                    case 5005: //讀卡機的就診日期時間讀取失敗
                    case 5006: //讀取安全模組內的「醫療院所代碼」失敗
                    case 5007: //寫入一組新的「就醫資料登錄」失敗
                    case 5008: //安全模組簽章失敗
                    case 5009: //投保單位無權限
                    case 5010: //同一天看診兩科(含)以上
                    case 5012: //此人未在保
                    case 5081: //最近24小時內同院所未曾就醫，故不可取消就醫（就醫類別輸入 ZA/ZB 時檢查）
                    case 9129: //持卡人於非所限制的醫療院所就診
                        break;
                }
            }
            return seqNumberData;
        }

        /// <summary>
        /// 更新健保卡內容
        /// </summary>
        public bool UpdateHcContents()
        {
            if (OpenCom())
            {
                var res = HisApiBase.csUpdateHCContents();
                ParseErrorCode(res);
                CloseCom();
                switch (res)
                {
                    case 0: //無任何錯誤。
                        return true;
                    case 4000: //讀卡機 timeout
                    case 4013: //未置入健保 IC 卡
                    case 4029: //IC 卡權限不足
                    case 4033: //所置入非健保 IC 卡
                    case 4050: //安全模組尚未與 IDC 認證
                    case 4061: //網路不通
                    case 4071: //健保 IC 卡與 IDC 認證失敗
                    case 5130: //更新健保 IC 卡內容失敗。
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// 取得門診處方箋
        /// </summary>
        /// <returns></returns>
        public HisPrescriptionData ReadPrescriptions()
        {
            if (OpenCom())
            {
                //門診處方箋(60組)
                var pOutpatientPrescription = new byte[3660];
                //byte[] pOutpatientPrescription = new byte[61];
                var iBufferLenOutpatient = 3660;
                //int iBufferLenOutpatient = 61;
                //長期處方箋(30組)
                var pLongTermPrescription = new byte[1320];
                //byte[] pLongTermPrescription = new byte[44];
                var iBufferLenLongTerm = 1320;
                //int iBufferLenLongTerm = 44;
                //重要醫令項目(10組)
                var pImportantTreatmentCode = new byte[360];
                //byte[] pImportantTreatmentCode = new byte[36];
                var iBufferLenImportant = 360;
                //int iBufferLenImportant = 36;
                //過敏藥物(3組)
                var pIrritationDrug = new byte[120];
                //byte[] pIrritationDrug = new byte[40];
                var iBufferLenIrritation = 120;
                //int iBufferLenIrritation = 40;

                var res = HisApiBase.hisReadPrescription(pOutpatientPrescription, ref iBufferLenOutpatient, pLongTermPrescription, ref iBufferLenLongTerm, pImportantTreatmentCode, ref iBufferLenImportant, pIrritationDrug, ref iBufferLenIrritation);
                ParseErrorCode(res);
                CloseCom();
                switch (res)
                {
                    case 0: //無任何錯誤
                        return new HisPrescriptionData(pOutpatientPrescription,pLongTermPrescription,pImportantTreatmentCode,pIrritationDrug);
                    case 4000: //讀卡機 timeout
                    case 4013: //未置入健保 IC 卡
                    case 4029: //IC 卡權限不足
                    case 4033: //所置入非健保 IC 卡
                    case 4050: //安全模組尚未與 IDC 認證
                        break;
                }
            }
            return new HisPrescriptionData();
        }

        /// <summary>
        /// 寫卡
        /// </summary>
        /// <returns></returns>
        public List<string> WritePrescriptionData(PrescriptionData p)
        {
            var signList = new List<string>();
            //var medList = p.Medicines.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf).ToList();
            var iWriteCount = GetDeclareMedicinesCount();
            var iBufferLength = 40 * iWriteCount;
            var treatDateTime = ConvertData.DateTimeToTaiwanCalendarString(new DateTime(), TimeUnit.Sec); //就診日期時間
            var pDateTime = ConvertData.StringToBytes(treatDateTime + "\0");
            var pPatientID = ConvertData.StringToBytes("身分證號或身分證明文件號碼" + "\0"); //身分證號或身分證明文件號碼
            var pPatientBirthDay = ConvertData.StringToBytes("生日" + "\0"); //生日
            var pDataWriteStr = CreateMedicalData();
            var pDataWrite = ConvertData.StringToBytes(pDataWriteStr);
            var pBuffer = new byte[iBufferLength];
            if (OpenCom())
            {
                var res = HisApiBase.hisWriteMultiPrescriptSign(pDateTime, pPatientID, pPatientBirthDay, pDataWrite,
                    ref iWriteCount, pBuffer, ref iBufferLength);
                ParseErrorCode(res);
                CloseCom();
                switch (res)
                {
                    case 0:
                        var startIndex = 0;
                        for (var i = 0; i < iWriteCount; i++)
                        {
                            signList.Add(ConvertData.BytesToString(pBuffer, startIndex, 40));
                            startIndex += 40;
                        }

                        break;
                    case 4000: //讀卡機 timeout
                    case 4013: //未置入健保 IC 卡
                    case 4029: //IC 卡權限不足
                    case 4033: //所置入非健保 IC 卡
                    case 4050: //安全模組尚未與 IDC 認證
                    case 5020: //要寫入的資料和健保 IC 卡不是屬於同一人。
                    case 5022: //找不到「就醫資料登錄」中的該組資料。
                    case 5033: //「門診處方箋」寫入失敗。
                    case 6018: //參數錯誤，pBuffer 之長度至少為 40 乘上 iWriteCount，CS 會對
                        break;
                }
            }

            return signList;

            //取得申報藥品數(健保藥.特材.虛擬醫令)
            int GetDeclareMedicinesCount()
            {
                return 0;
            }

            //產生寫卡資料
            string CreateMedicalData()
            {
                /* 每個申報藥品產生資料格式 :
                     就診日期時間(1-13)
                     醫令類別(14)
                     診療項目代號(15-26)
                     診療部位(27-32)
                     用法(33-50)
                     天數(51-52)
                     總量(53-59)
                     交付處方註記(60-61)
                */ 
                return string.Empty;
            }
        }

        /// <summary>
        /// 開啟讀卡機
        /// </summary>
        /// <returns></returns>
        private bool OpenCom(int com = 0)
        {
            var res = HisApiBase.csOpenCom(com);
            ParseErrorCode(res);
            return res == 0;
        }

        /// <summary>
        /// 關閉讀卡機
        /// </summary>
        private void CloseCom()
        {
            HisApiBase.csCloseCom();
        }

        /// <summary>
        /// 認證安全模組
        /// </summary>
        /// <returns></returns>
        public bool VerifySamDc()
        {
            if (OpenCom())
            {
                var res = HisApiBase.csVerifySAMDC();
                CloseCom();
                return res == 0;
            }

            return false;
        }


        /// <summary>
        /// 驗證醫事人員卡
        /// </summary>
        /// <returns></returns>
        public bool VerifyHpcPin()
        {
            if (OpenCom())
            {
                var res = HisApiBase.hpcVerifyHPCPIN();
                CloseCom();
                return res == 0;
            }

            return false;
        }

        /// <summary>
        /// 每日上傳
        /// </summary>
        /// <returns></returns>
        public bool DailyUpload(string uploadFilePath, string recCount)
        {
            var fileName = uploadFilePath;
            var fileNameArr = ConvertData.StringToBytes(fileName);
            var fileInfo = new FileInfo(fileName); //每日上傳檔案
            var fileSize = ConvertData.StringToBytes(fileInfo.Length.ToString()); //檔案大小
            var count = ConvertData.StringToBytes(recCount);
            var pBuffer = new byte[50];
            var iBufferLength = 50;
            if (OpenCom())
            {
                var res = HisApiBase.csUploadData(fileNameArr, fileSize, count, pBuffer, ref iBufferLength);
                CloseCom();
                if (res != 0)
                    return false;
                var samCode = ConvertData.BytesToString(pBuffer, 0, 12);
                var insID = ConvertData.BytesToString(pBuffer, 12, 10);
                var uploadDateTime = ConvertData.BytesToString(pBuffer, 22, 14);
                var uploadYear = uploadDateTime.Substring(0, 4);
                var uploadMonth = uploadDateTime.Substring(4, 2);
                var uploadDay = uploadDateTime.Substring(6, 2);
                var uploadHour = uploadDateTime.Substring(8, 2);
                var uploadMinute = uploadDateTime.Substring(10, 2);
                var uploadSecond = uploadDateTime.Substring(12, 2);
                var uploadDateStr =
                    $"{uploadYear}/{uploadMonth}/{uploadDay} {uploadHour}:{uploadMinute}:{uploadSecond}";
                var receiveDateTime = ConvertData.BytesToString(pBuffer, 36, 14);
                var receiveYear = receiveDateTime.Substring(0, 4);
                var receiveMonth = receiveDateTime.Substring(4, 2);
                var receiveDay = receiveDateTime.Substring(6, 2);
                var receiveHour = receiveDateTime.Substring(8, 2);
                var receiveMinute = receiveDateTime.Substring(10, 2);
                var receiveSecond = receiveDateTime.Substring(12, 2);
                var receiveDateStr =
                    $"{receiveYear}/{receiveMonth}/{receiveDay} {receiveHour}:{receiveMinute}:{receiveSecond}";
                var uploadTime = new DateTime(int.Parse(uploadYear), int.Parse(uploadMonth),
                    int.Parse(uploadDay), int.Parse(uploadHour), int.Parse(uploadMinute),
                    int.Parse(uploadSecond));
                var receiveTime = new DateTime(int.Parse(receiveYear), int.Parse(receiveMonth),
                    int.Parse(receiveDay), int.Parse(receiveHour), int.Parse(receiveMinute),
                    int.Parse(receiveSecond));
                return true;
            }

            return false;
        }

        private static void ParseErrorCode(int res)
        {
            if (Enum.IsDefined(typeof(CsHisAPIErrorCode), res))
                ErrorCode = (CsHisAPIErrorCode) res;
        }

        public static string GetErrorCodeDescription()
        {
            return ErrorCode.GetDescription();
        }

        public static void TestErrorStatus()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var errorCode in (CsHisAPIErrorCode[]) Enum.GetValues(typeof(CsHisAPIErrorCode)))
                {
                    ErrorCode = errorCode;
                    Thread.Sleep(200);
                }
            });
        }
    }
    public class HisAPITest : IHISAPI
    {
        public static CsHisAPIErrorCode ErrorCode = CsHisAPIErrorCode.Success;
        /// <summary>
        /// 取得基本資料
        /// </summary>
        /// <returns></returns>
        public IcCardData GetBasicData()
        {
            return new IcCardData("000012345678", "AAdc", "T222222222",new DateTime(2000,01,02),Gender.男, new DateTime(2000, 02, 02),"1","0912345678");
        }

        /// <summary>
        /// 取得就醫序號
        /// </summary>
        /// <returns></returns>
        public SeqNumber GetSeqNumber(byte bTreatAfterCheck, string babyTreat = " ")
        {
            return new SeqNumber();
        }

        /// <summary>
        /// 更新健保卡內容
        /// </summary>
        public bool UpdateHcContents()
        {
            return true;
        }

        HisPrescriptionData IHISAPI.ReadPrescriptions()
        {
            return ReadPrescriptions();
        }

        /// <summary>
        /// 取得門診處方箋
        /// </summary>
        /// <returns></returns>
        private HisPrescriptionData ReadPrescriptions()
        {
            return new HisPrescriptionData();
        }

        /// <summary>
        /// 寫卡
        /// </summary>
        /// <returns></returns>
        public List<string> WritePrescriptionData(PrescriptionData p)
        {
            return new List<string>();
        }

        /// <summary>
        /// 認證安全模組
        /// </summary>
        /// <returns></returns>
        public bool VerifySamDc()
        {
            return true;
        }

        /// <summary>
        /// 驗證醫事人員卡
        /// </summary>
        /// <returns></returns>
        public bool VerifyHpcPin()
        {
            return true;
        }

        public bool DailyUpload(string uploadFilePath, string recCount)
        {
            return true;
        }
    }
}