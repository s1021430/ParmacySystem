using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.Prescription.MedicalBaseClass;

namespace PharmacySystemInfrastructure.Prescription
{
    public class DiseaseCodeDatabaseRepository : IDiseaseCodeRepository
    {
        public DiseaseCodeDatabaseRepository() { }

        public DiseaseCode GetDiseasesById(string id)
        {
            DiseaseCode result = new DiseaseCode();
            var getDiseaseCode = $@"Select ID,ChineseName,EnglishName,Type
                                from NHI.DiseaseCode
                                where ID = '{id}';";
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();
                result = conn.QueryFirstOrDefault<DiseaseCode>(getDiseaseCode);
                conn.Close();
            }

            return result;
        }

        public List<DiseaseCode> SearchDiseasesById(string id)
        {
            var result = new List<DiseaseCode>();
            string getDiseaseCode = $@"Select ID,ChineseName,EnglishName,Type
                                from NHI.DiseaseCode
                                where ID Like '%{id}%';";
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();
                result = conn.Query<DiseaseCode>(getDiseaseCode).ToList();
                conn.Close();
            }

            return result;
        }
    }
}
