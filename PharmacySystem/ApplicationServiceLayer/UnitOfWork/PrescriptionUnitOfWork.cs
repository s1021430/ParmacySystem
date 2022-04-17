using System;
using System.Data.SqlClient;
using System.Transactions;
using GeneralClass;
using GeneralClass.Prescription;
using PharmacySystemInfrastructure.Prescription;
using PharmacySystemInfrastructure.Stock;

namespace PharmacySystem.ApplicationServiceLayer.UnitOfWork
{
    public class PrescriptionUnitOfWork : IDisposable
    {
        private readonly TransactionScope transaction;
        private readonly SqlConnection connection;

        public IPrescriptionRepository PrescriptionRepository { get; }
        public StockApplicationService StockService { get; }

        public PrescriptionUnitOfWork()
        {
            transaction = new TransactionScope();
            connection = new SqlConnection(DBInvoker.ConnectionString);
            PrescriptionRepository = new PrescriptionDataBaseRepository(connection);
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
