using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalOrders;

namespace PharmacySystemInfrastructure.Prescription
{
    public class MedicalOrderDatabaseRepository : IMedicalOrderRepository
    {
        private static readonly List<MedicalOrder> VirtualMedicalOrders = new()
        {
            new VirtualOrder("R001", "因處方箋遺失或毀損，提供切結文件，提前回診，且經院所查詢健保雲端藥歷系統，確定病人未領取所稱遺失或毀損處方之藥品。"),
            new VirtualOrder("R002", "因醫師請假因素，提前回診，醫事服務機構留存醫師請假證明資料備查。"),
            new VirtualOrder("R003", "經醫師專業認定需要改藥或調整藥品劑量或換藥者。"),
            new VirtualOrder("R004", "其他非屬 R001~R003 之提前回診或慢性病連續處方箋提前領取藥品或其他等病人因素，提供切結文件或於病歷中詳細記載原因備查。"),
            new VirtualOrder("R005", "民眾健保卡加密或其他健保卡問題致無法查詢健保雲端資訊，並於病歷中記載原因備查"),
            new VirtualOrder("R006", "醫院轉出(或回轉)病人至診所第 1 次就醫且符合轉診申報規定，經查詢雲藥系統有餘藥，已向病人衛教並於病歷中記載原因備查後處方。"),
            new VirtualOrder("R007", "配合衛福部食品藥物管理署公告藥品回收，重新開立處方給病人，並於病歷中記載原因備查。"),
            new VirtualOrder("R008", "醫師查詢雲端或 API 系統提示病人有重複用藥情事，經向病人確認後排除未領藥紀錄，其餘藥天數小於(含)10 天開立處方，並於病歷中詳細記載原因備查。")
        };

        private const string MedicineQueryBase =
            "SELECT * FROM NHI.Medicine MED,NHI.MedicinePrice PRICE WHERE MED.ID = PRICE.ID AND PRICE.EndDate > SYSDATETIME()";
        private const string SpecialMaterialQueryBase =
            "SELECT * FROM NHI.SpecialMedicine ,NHI.SpecialMedicinePrice PRICE WHERE ID = SPEMED_ID AND PRICE.EndDate > SYSDATETIME()";
        public List<MedicalOrder> SearchMedicalOrdersByID(string id)
        {
            var result = new List<MedicalOrder>();

            string getSpecialMed = $@"Select ID as Id,ChineseName,EnglishName,Price
                            from NHI.SpecialMedicine PRICE,NHI.SpecialMedicinePrice
                            where ID = SPEMED_ID and 
                            	EndDate > SYSDATETIME() and
                            	ID like '%{id}%';";

            string getMed = $@"Select med.ID As Id,ChineseName,EnglishName,Price
                            from NHI.Medicine med,NHI.MedicinePrice medprice
                            where med.ID = medprice.ID and 
                            	medprice.EndDate > SYSDATETIME() and
                            	med.ID like '%{id}%';";

            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();
                result.AddRange(conn.Query<MedicineOrder>(getMed).ToList());
                result.AddRange(conn.Query<SpecialMaterialOrder>(getSpecialMed).ToList());
                conn.Close();
            }

            return result;
        }

        public List<MedicalOrder> GetMedicalOrdersByID(List<MedicalOrderID> medicalOrdersID)
        {
            var result = new List<MedicalOrder>();
            var ordersID = string.Join(",", medicalOrdersID.Select(o => $"'{o.ID}'"));
            var queryMedicine = $@"{MedicineQueryBase} AND MED.ID IN ({ordersID})";
            var querySpecialMaterial = $@"{SpecialMaterialQueryBase} AND ID IN ({ordersID})";
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            result.AddRange(conn.Query<MedicineOrder>(queryMedicine).ToList());
            result.AddRange(conn.Query<SpecialMaterialOrder>(querySpecialMaterial).ToList());
            conn.Close();
            return result;
        }

        public List<MedicalOrderID> GetControlMedicine()
        {
            var sql = "  Select MED_ID ID from NHI.ControlMedicine;";

            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            var result = conn.Query<MedicalOrderID>(sql).ToList();
            conn.Close();
            return result;

        }

        public MedicalOrder GetVirtualOrder(string id)
        {
            return VirtualMedicalOrders.SingleOrDefault(_ => _.ID.ID.Equals(id));
        }
    }
}
