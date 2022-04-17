using System;
using System.Data.SqlClient;
using System.Transactions;
using GeneralClass;
using GeneralClass.PurchaseOrder;
using PharmacySystemInfrastructure.Stock;
using PharmacySystemInfrastructure.StoreOrder;

namespace PharmacySystem.ApplicationServiceLayer.UnitOfWork
{
    public class PurchaseOrderUnitOfWork : IDisposable
    {
        private readonly TransactionScope transaction;
        private readonly SqlConnection connection;

        public IPurchaseOrderRepository PurchaseOrderRepository { get; }
        public StockApplicationService StockService { get; }

        public PurchaseOrderUnitOfWork()
        {
            transaction = new TransactionScope();
            connection = new SqlConnection(DBInvoker.ConnectionString);

            PurchaseOrderRepository = new PurchaseOrderDatabaseRepository(connection);

            var stockRepository = new StockRepository(connection);
            StockService = new StockApplicationService(stockRepository);

            connection.Open();
        }


        public void Commit()
        {
            transaction.Complete();
            connection.Close();
        }

        public void Dispose()
        {
            transaction?.Dispose();
            connection?.Dispose();
        }
    }
}
