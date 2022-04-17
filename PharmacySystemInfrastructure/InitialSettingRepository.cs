using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using GeneralClass;
using GeneralClass.SystemInfo;

namespace PharmacySystemInfrastructure
{
    public class InitialSettingRepository
    {

        public bool IsNeedInit()
        {
            string sqlstring = " Select Count(*) count from Main.SysInfo.InitSetting;";
            using (SqlConnection conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                Task checkConnectedTask = new Task(() =>
                {
                    conn.Open();
                });
                checkConnectedTask.Start();
                var result = checkConnectedTask.Wait(5000);
                if (result == false)
                    return true;

                int settingCount = conn.QueryFirst<int>(sqlstring);
                conn.Close();

                return settingCount == 0;
            }

        }
        public bool RegisterData(InitialSettingData data)
        {
            string sqlConnectingString = $@"Database=Main;Server={data.IP};User Id=PS_Admin;Password=city1234;";
            using (SqlConnection conn = new SqlConnection(sqlConnectingString))
            {

                Task checkConnectedTask = new Task(() =>
                {
                    conn.Open();
                });
                checkConnectedTask.Start();
                if (checkConnectedTask.Wait(5000) == false)
                    return false;

                DBInvoker.ConnectionString = sqlConnectingString;
                conn.Execute(@"Update Main.SysInfo.InitSetting Set [EndDate] = SYSDATETIME() where [EndDate] = null;");

                int needInsert = conn.QueryFirst<int>(@"Select Count(*) count
                                from Main.SysInfo.InitSetting
                                where EndDate is null and Pharmacy_Code = @PharmacyCode;", data);

                if (needInsert == 0)
                    conn.Execute(@" Insert into Main.SysInfo.InitSetting (Pharmacy_Code, StartDate, EndDate) Values(@PharmacyCode, SYSDATETIME(), null);", data);

                conn.Close();
                return true;
            }
        }
    }
}
