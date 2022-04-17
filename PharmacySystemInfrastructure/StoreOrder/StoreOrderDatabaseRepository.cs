using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.PurchaseOrder;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseOrder.SearchCondition;
using GeneralClass.PurchaseRequisition.EntityIndex;

namespace PharmacySystemInfrastructure.StoreOrder
{
    public class PurchaseOrderDatabaseRepository : IPurchaseOrderRepository
    {
        private readonly SqlConnection connection;

        private readonly string[] purchaseOrderColumns = {"ID", "RequisitionID", "ManufactoryID", "WareHouseID", "Status", "CompleteTime", "Note", "CustomerID" };
        private readonly string[] purchaseOrderProductColumns = { "STOORD_ID", "PRO_ID", "SerialNumber", "Amount", "Unit", "UnitAmount", "FreeAmount", "Price", "SubTotal", "ValidDate", "BatchNumber", "Note" };

        public PurchaseOrderDatabaseRepository(){}

        public PurchaseOrderDatabaseRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public PurchaseOrder GetPurchaseOrderByID(PurchaseOrderID orderID)
        {
            throw new NotImplementedException();
        }
        
        public bool Save(PurchaseOrder order)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = SavePurchaseOrderByConnection(localConnection, order);

                localConnection.Close();
            }
            else
                result = SavePurchaseOrderByConnection(connection, order);

            return result;
        }

        private bool SavePurchaseOrderByConnection(SqlConnection localConnection, PurchaseOrder order)
        {
            var localTransaction = localConnection.BeginTransaction();

            try
            {
                var purchaseOrderSql = $"UPDATE StoreOrder.PurchaseOrder SET {DBInvoker.GetUpdateTableColumns(purchaseOrderColumns.Skip(1).ToArray())} WHERE ID = {order.ID.ID}";
                var deleteOrderProductSql = $"DELETE StoreOrder.PurchaseOrderProduct WHERE STOORD_ID = {order.ID.ID}";
                var insertOrderProductSql = $"INSERT INTO StoreOrder.PurchaseOrderProduct({DBInvoker.GetTableColumns(purchaseOrderProductColumns)}) VALUES ({DBInvoker.GetTableParameterColumns(purchaseOrderProductColumns)})";

                localConnection.Execute(purchaseOrderSql, GetPurchaseOrderDAO(order));
                localConnection.Execute(deleteOrderProductSql);
                localConnection.Execute(insertOrderProductSql, GetPurchaseOrderProductDAO(order));
                
                localTransaction.Commit();
            }
            catch (Exception e)
            {
                // log exception
                localTransaction.Rollback();
                return false;
            }

            return true;
        }

        public bool Create(PurchaseOrder order)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = CreatePurchaseOrderByConnection(localConnection, order);

                localConnection.Close();
            }
            else
                result = CreatePurchaseOrderByConnection(connection, order);

            return result;
        }

        private bool CreatePurchaseOrderByConnection(SqlConnection localConnection, PurchaseOrder order)
        {
            try
            {
                var purchaseOrderSql = $"INSERT INTO StoreOrder.PurchaseOrder({DBInvoker.GetTableColumns(purchaseOrderColumns)}) VALUES ({DBInvoker.GetTableParameterColumns(purchaseOrderColumns)})";
                localConnection.Execute(purchaseOrderSql, GetPurchaseOrderDAO(order));
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool Delete(PurchaseOrderID orderID)
        {
            bool result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = DeletePurchaseOrderByConnection(localConnection, orderID);

                localConnection.Close();
            }
            else
                result = DeletePurchaseOrderByConnection(connection, orderID);

            return result;
        }

        private bool DeletePurchaseOrderByConnection(SqlConnection localConnection, PurchaseOrderID orderID)
        {
            try
            {
                var sql = $"UPDATE StoreOrder.PurchaseOrder SET IsEnable = 0 WHERE ID = {orderID.ID}";
                localConnection.Execute(sql);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        
        public List<PurchaseOrder> GetDraftPurchaseOrders()
        {
            List<PurchaseOrder> result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = GetDraftPurchaseOrdersByConnection(localConnection);

                localConnection.Close();
            }
            else
                result = GetDraftPurchaseOrdersByConnection(connection);

            return result;
        }

        private List<PurchaseOrder> GetDraftPurchaseOrdersByConnection(SqlConnection localConnection)
        {
            var sql = "SELECT PO.*, PO_P.*, PO_P.PRO_ID AS ID " +
                      "FROM StoreOrder.PurchaseOrder AS PO LEFT JOIN StoreOrder.PurchaseOrderProduct AS PO_P ON PO.ID = PO_P.STOORD_ID " +
                      "WHERE Status = 1";

            var orderDictionary = new Dictionary<PurchaseOrderID, PurchaseOrder>();

            var result = localConnection.Query<PurchaseOrder, OrderProduct, PurchaseOrder>(
                sql,
                (order, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.ID, out PurchaseOrder orderEntry))
                    {
                        orderEntry = order;
                        orderDictionary.Add(orderEntry.ID, orderEntry);
                    }

                    if (product.ID.IsValid())
                        orderEntry.Products.Add(product);

                    return orderEntry;
                },
                splitOn: "STOORD_ID").Distinct().ToList();

            return result;
        }

        public List<PurchaseOrder> GetCompletePurchaseOrders(PurchaseOrderSearchCondition searchCondition)
        {
            List<PurchaseOrder> result;

            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();

                result = GetCompletePurchaseOrdersByConnection(localConnection, searchCondition);

                localConnection.Close();
            }
            else
                result = GetCompletePurchaseOrdersByConnection(connection, searchCondition);

            return result;
        }

        private List<PurchaseOrder> GetCompletePurchaseOrdersByConnection(SqlConnection localConnection, PurchaseOrderSearchCondition searchCondition)
        {
            var sql = "SELECT PO.*, PO_P.*, PO_P.PRO_ID AS ID " +
                      "FROM StoreOrder.PurchaseOrder AS PO LEFT JOIN StoreOrder.PurchaseOrderProduct AS PO_P ON PO.ID = PO_P.STOORD_ID " +
                      "WHERE Status = 2";

            sql += GetSearchCondition();

            return localConnection.Query<PurchaseOrder>(sql).ToList();

            string GetSearchCondition()
            {
                var searchSqlCondition = string.Empty;

                if (searchCondition.ID != null)
                    searchSqlCondition += $"AND ID = {searchCondition.ID.Value.ID}";

                if (searchCondition.RequisitionID != null)
                    searchSqlCondition += $"AND RequisitionID = {searchCondition.RequisitionID.Value.ID}";

                if (searchCondition.ManufactoryID != null)
                    searchSqlCondition += $"AND ManufactoryID = {searchCondition.ManufactoryID.Value.ID}";

                if (searchCondition.WareHouseID != null)
                    searchSqlCondition += $"AND WareHouseID = {searchCondition.WareHouseID.Value.ID}";

                if (searchCondition.CustomerID != null)
                    searchSqlCondition += $"AND CustomerID = {searchCondition.CustomerID.Value.ID}";
                
                if (searchCondition.ProductID != null)
                    searchSqlCondition += $"AND ID IN (SELECT STOORD_ID FROM StoreOrder.PurchaseOrderProduct WHERE PRO_ID = '{searchCondition.ProductID.Value.ID}')";

                return searchSqlCondition;
            }
        }
        
        public PurchaseOrderID GetNewPurchaseOrderID()
        {
            var randomID = GenerateRandomID();
            return new PurchaseOrderID($"PO-{DateTime.Today:yyyyMMdd}-{randomID}");
        }

        private string GenerateRandomID()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
        }

        private object GetPurchaseOrderDAO(PurchaseOrder order)
        {
            return new
            {
                ID = order.ID.ID,
                RequisitionID = order.RequisitionID.ID,
                ManufactoryID = order.ManufactoryID.ID,
                WareHouseID = order.WareHouseID.ID,
                Status = (int)order.Status,
                CompleteTime = order.CompleteTime,
                Note = order.Note,
                CustomerID = order.CustomerID.ID
            };
        }

        private object[] GetPurchaseOrderProductDAO(PurchaseOrder order)
        {
            var productDAOList = new List<object>();

            var index = 0;

            foreach (var product in order.Products)
            {
                var productDAO = new
                {
                    STOORD_ID = order.ID.ID,
                    PRO_ID = product.ID.ID,
                    SerialNumber = index,
                    Amount = product.Amount,
                    Unit = 0,
                    UnitAmount = 0,
                    FreeAmount = product.FreeAmount,
                    Price = product.Price,
                    SubTotal = product.SubTotal,
                    ValidDate = product.ValidDate,
                    BatchNumber = product.BatchNumber,
                    Note = product.Note
                };

                productDAOList.Add(productDAO);
                index++;
            }

            return productDAOList.ToArray();
        }
    }
}
