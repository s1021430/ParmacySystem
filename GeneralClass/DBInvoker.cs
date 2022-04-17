using System.Linq;

namespace GeneralClass
{
    public class DBInvoker
    { 
        public static string ConnectionString = "Database=Main;Server=36.228.173.115,20026;User Id=PS_Admin;Password=city1234;";

        public static string GetTableColumns(string[] columns)
        {
            return string.Join(",", columns);
        }

        public static string GetTableParameterColumns(string[] columns)
        {
            return "@" + string.Join(",@", columns);
        }

        public static string GetUpdateTableColumns(string[] columns)
        {
            var updateString = columns.Aggregate("", (current, column) => current + $"{column} = @{column},");

            return updateString.Trim(',');
        }
    }
}
