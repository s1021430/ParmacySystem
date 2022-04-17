using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Transaction;
namespace DataBaseConnector
{
    public static class MSSQLConnector
    {
        public const string strConnection = "AAAA";

        ///一次撈一張Table
        public static List<dynamic> Query( SQLInfo sqlInfo)
        { 
            using (SqlConnection conn = new SqlConnection(strConnection))
            { 
                return conn.Query(sqlInfo.StrSQL, new DynamicParameters(sqlInfo.SQLParam)).ToList();
            } 
        }
         
        ///一次撈多張Table, strSQL用;分離, sqlparam共用一組
        public static List<List<dynamic>> MultiQuery(SQLInfo sqlInfo)
        {
            List<List<dynamic>> result = new List<List<dynamic>>();
            
            using (SqlConnection conn = new SqlConnection(strConnection))
            { 
                using (var results = conn.QueryMultiple(sqlInfo.StrSQL, new DynamicParameters(sqlInfo.SQLParam)))
                {
                    IEnumerable<dynamic> data;
                    while ( (data = results.Read()) != null)
                    {
                        result.Add(data.ToList());
                    }  
                }
            } 
            return result;
        }

        public static bool ExecuteWithTransaction(List<SQLInfo> sqlInfoList)
        {
            using (SqlConnection conn = new SqlConnection(strConnection))
            {
                conn.Open(); 
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var sqlInfo in sqlInfoList)
                        {
                            conn.Execute(sqlInfo.StrSQL, sqlInfo.SQLParam, transaction: trans);
                        }
                        trans.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return false;
                    } 
                } 
            } 
        }
    }
}
