using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.Stock;

namespace PharmacySystemInfrastructure.Stock
{
    public class StockRepository
    {
        private readonly SqlConnection connection;

        public SqlConnection GetConn()
        {
            return connection;
        }

        public StockRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void SaveRecord(List<InventoryRecord> dataList, SqlConnection conn)
        {

            if (conn == null)
            {
                using (conn = new SqlConnection(DBInvoker.ConnectionString))
                {
                    conn.Open();

                    try
                    {
                        conn.Execute($@"Insert into Product.InventoryRecord 
                    ({inventoryRecordColumn})
                    Values({inventoryRecordParamColumn})", dataList);
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                conn.Execute($@"Insert into Product.InventoryRecord 
                    ({inventoryRecordColumn})
                    Values({inventoryRecordParamColumn})", dataList);
            }

        }

        public List<InventoryRecord> GetStockRecord(int invID, SqlConnection conn)
        {
            if (conn == null)
            {
                using (conn = new SqlConnection(DBInvoker.ConnectionString))
                {
                    conn.Open();

                    try
                    {
                        return conn.Query<InventoryRecord>(
                            $@"Select {inventoryRecordColumn} 
                    from Product.InventoryRecord
                    where InvID = {invID}
                    order by ExecuteTime; ").ToList();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                return conn.Query<InventoryRecord>(
                    $@"Select {inventoryRecordColumn} 
                    from Product.InventoryRecord
                    where InvID = {invID}
                    order by ExecuteTime; ").ToList();
            }
        }

        public int GetInvIDByProIDandWareID(string proID, int wareID, SqlConnection conn)
        {
            string sql = $@"Select INV_ID
                            from Product.InventoryMapping
                            where PRO_ID='{proID}' and WARE_ID='{wareID}';";

            if (conn == null)
            {
                using (conn = new SqlConnection(DBInvoker.ConnectionString))
                {

                    conn.Open();
                    var invID = conn.QueryFirstOrDefault<int>(sql);
                    conn.Close();
                    return invID;
                }
            }
            else
            {
                return conn.QueryFirstOrDefault<int>(sql);
            }

        }

        public List<int> GetAllWareID(SqlConnection conn)
        {
            string sql = $@"Select ID
                                from Product.WareHouse 
                                where IsEnable = 1;";
            if (conn == null)
            {
                using (conn = new SqlConnection(DBInvoker.ConnectionString))
                {

                    conn.Open();
                    var wareIDList = conn.Query<int>(sql).ToList();
                    conn.Close();
                    return wareIDList;
                }
            }
            else
            {
                return conn.Query<int>(sql).ToList();
            }
        }


        const string inventoryRecordColumn = @"Type, InvID, Amount, TotalValue, SourceID, BatchNumber, ExecuteTime,ExecuteEmployeeID ";

        const string inventoryRecordParamColumn = @"@Type, @InvID, @Amount, @TotalValue, @SourceID, @BatchNumber, @ExecuteTime, @ExecuteEmployeeID";
    }
}
