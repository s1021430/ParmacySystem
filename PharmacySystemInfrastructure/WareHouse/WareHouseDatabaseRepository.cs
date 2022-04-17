using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using GeneralClass;
using GeneralClass.WareHouse;
using GeneralClass.WareHouse.EntityIndex;

namespace PharmacySystemInfrastructure.WareHouse
{
    public class WareHouseDatabaseRepository : IWareHouseRepository
    {
        private readonly SqlConnection connection;

        private readonly string[] wareHouseColumns = { "ID", "Name", "IsEnable" };

        public WareHouseDatabaseRepository() {}

        public WareHouseDatabaseRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public GeneralClass.WareHouse.WareHouse GetWareHouseByID(WareHouseID wareHouseID)
        {
            GeneralClass.WareHouse.WareHouse result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = GetWareHouseByConnection(localConnection, wareHouseID);

                localConnection.Close();
            }
            else
                result = GetWareHouseByConnection(connection, wareHouseID);

            return result;
        }

        private GeneralClass.WareHouse.WareHouse GetWareHouseByConnection(SqlConnection connection, WareHouseID wareHouseID)
        {
            var sql = $"SELECT * FROM Product.WareHouse WHERE ID = {wareHouseID.ID}";
            return connection.Query<GeneralClass.WareHouse.WareHouse>(sql).First();
        }

        public bool Save(GeneralClass.WareHouse.WareHouse wareHouse)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = SaveWareHouseByConnection(localConnection, wareHouse);

                localConnection.Close();
            }
            else
                result = SaveWareHouseByConnection(connection, wareHouse);

            return result;
        }

        private bool SaveWareHouseByConnection(SqlConnection localConnection, GeneralClass.WareHouse.WareHouse wareHouse)
        {
            try
            {
                var wareHouseSql = $"UPDATE Product.WareHouse SET {DBInvoker.GetUpdateTableColumns(wareHouseColumns.Skip(1).ToArray())} WHERE ID = {wareHouse.ID.ID}";
                localConnection.Execute(wareHouseSql, GetWareHouseDAO(wareHouse));
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool Create(GeneralClass.WareHouse.WareHouse wareHouse)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = CreateWareHouseByConnection(localConnection, wareHouse);

                localConnection.Close();
            }
            else
                result = CreateWareHouseByConnection(connection, wareHouse);

            return result;
        }

        private bool CreateWareHouseByConnection(SqlConnection localConnection, GeneralClass.WareHouse.WareHouse wareHouse)
        {
            try
            {
                var wareHouseSql = $"UPDATE Product.WareHouse SET {DBInvoker.GetUpdateTableColumns(wareHouseColumns.Skip(1).ToArray())} WHERE ID = {wareHouse.ID.ID}";
                localConnection.Execute(wareHouseSql, GetWareHouseDAO(wareHouse));
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool Delete(WareHouseID wareHouseID)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = DeleteWareHouseByConnection(localConnection, wareHouseID);

                localConnection.Close();
            }
            else
                result = DeleteWareHouseByConnection(connection, wareHouseID);

            return result;
        }

        private bool DeleteWareHouseByConnection(SqlConnection localConnection, WareHouseID wareHouseID)
        {
            try
            {
                var sql = $"UPDATE Product.WareHouse SET IsEnable = 0 WHERE ID = {wareHouseID.ID}";
                localConnection.Execute(sql);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public List<GeneralClass.WareHouse.WareHouse> GetAllWareHouses()
        {
            List<GeneralClass.WareHouse.WareHouse> result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = GetWareHousesByConnection(localConnection);

                localConnection.Close();
            }
            else
                result = GetWareHousesByConnection(connection);

            return result;
        }

        private List<GeneralClass.WareHouse.WareHouse> GetWareHousesByConnection(SqlConnection localConnection)
        {
            var sql = "SELECT * FROM Product.WareHouse";
            return localConnection.Query<GeneralClass.WareHouse.WareHouse>(sql).ToList();
        }

        private object GetWareHouseDAO(GeneralClass.WareHouse.WareHouse wareHouse)
        {
            return new
            {
                ID = wareHouse.ID.ID,
                Name = wareHouse.Name,
                IsEnable = wareHouse.IsEnable
            };
        }
    }
}
