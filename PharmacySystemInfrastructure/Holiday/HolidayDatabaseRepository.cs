using System;
using System.Data.SqlClient;
using Dapper;
using GeneralClass;
using GeneralClass.Common;

namespace PharmacySystemInfrastructure.Holiday
{
    public class HolidayDatabaseRepository: IHolidayRepository
    {
        public int GetHolidayCountBetweenDays(DateTime startDate,DateTime endDate)
        {
            var sql = $@"select Count(*) count
            from dbo.HolidayRecord
            where Holidate >= '{startDate:MM/dd/yyyy}' and Holidate <= '{endDate:MM/dd/yyyy}';";
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            var result = conn.QueryFirst<int>(sql);
            conn.Close();
            return result;
        }
    }
}
