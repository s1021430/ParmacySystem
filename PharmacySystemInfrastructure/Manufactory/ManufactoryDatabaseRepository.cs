using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.Manufactory;
using GeneralClass.Manufactory.EntityIndex;
using GeneralClass.Manufactory.SearchCondition;

namespace PharmacySystemInfrastructure.Manufactory
{
    public class ManufactoryDatabaseRepository : IManufactoryRepository
    {
        private readonly SqlConnection connection;

        private readonly string[] manufactoriesColumns = { "ID", "Name", "Abbreviation", "Note", "IsEnable" };

        public List<GeneralClass.Manufactory.Manufactory> GetAllManufactories()
        {
            List<GeneralClass.Manufactory.Manufactory> result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = GetManufactoryByConnection(localConnection);

                localConnection.Close();
            }
            else
                result = GetManufactoryByConnection(connection);

            return result;
        }

        private List<GeneralClass.Manufactory.Manufactory> GetManufactoryByConnection(SqlConnection connection)
        {
            var sql = "SELECT * FROM Product.Manufactory";
            return connection.Query<GeneralClass.Manufactory.Manufactory>(sql).ToList();
        }

        public bool Save(GeneralClass.Manufactory.Manufactory manufactory)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = SaveManufactoryByConnection(localConnection, manufactory);

                localConnection.Close();
            }
            else
                result = SaveManufactoryByConnection(connection, manufactory);

            return result;
        }

        private bool SaveManufactoryByConnection(SqlConnection localConnection, GeneralClass.Manufactory.Manufactory manufactory)
        {
            try
            {
                var sql = $"UPDATE Product.Manufactory SET {DBInvoker.GetUpdateTableColumns(manufactoriesColumns.Skip(1).ToArray())} WHERE ID = {manufactory.ID.ID}";
                localConnection.Execute(sql, GetManufactoryDAO(manufactory));
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool Create(GeneralClass.Manufactory.Manufactory manufactory)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ManufactoryID id)
        {
            throw new NotImplementedException();
        }

        public List<GeneralClass.Manufactory.Manufactory> GetManufactories(ManufactorySearchCondition searchCondition)
        {
            throw new NotImplementedException();
        }

        private object GetManufactoryDAO(GeneralClass.Manufactory.Manufactory manufactory)
        {
            return new
            {
                ID = manufactory.ID.ID,
                Name = manufactory.Name,
                Abbreviation = manufactory.Abbreviation,
                Note = manufactory.Note,
                IsEnable = manufactory.IsEnable
            };
        }
    }
}
