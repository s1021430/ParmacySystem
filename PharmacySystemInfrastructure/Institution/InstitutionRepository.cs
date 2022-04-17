using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.Prescription.MedicalBaseClass;

namespace PharmacySystemInfrastructure.Institution
{
    public class InstitutionDatabaseRepository : IInstitutionRepository
    {
        public List<GeneralClass.Prescription.MedicalBaseClass.Institution> GetInstitutionByID(string id)
        {
            var sql = GetSearchInstitutionsQueryString(id);
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            var result = conn.Query<GeneralClass.Prescription.MedicalBaseClass.Institution>(sql).ToList();
            conn.Close();
            return result;
        }

        public List<GeneralClass.Prescription.MedicalBaseClass.Institution> GetInstitutionByName(string name)
        {
            var sql = GetSearchInstitutionsQueryString(name);
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            var result = conn.Query<GeneralClass.Prescription.MedicalBaseClass.Institution>(sql).ToList();
            conn.Close();
            return result;
        }

        public List<GeneralClass.Prescription.MedicalBaseClass.Institution> GetInstitutionsByMultipleId(IEnumerable<string> institutionsID)
        {
            var ids = institutionsID.Select(ins => "\'" + ins + "\'").Distinct();
            var sql = $"SELECT * FROM NHI.Institution WHERE ID IN ({string.Join(",", ids)})";
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            var result = conn.Query<GeneralClass.Prescription.MedicalBaseClass.Institution>(sql).ToList();
            return result;
        }

        private static string GetSearchInstitutionsQueryString(string condition)
        {
            return $@"SELECT ID,Name,Address,Telephone,EndContractDate FROM NHI.Institution WHERE ID LIKE '%{condition}%';";
        }
    }
}
