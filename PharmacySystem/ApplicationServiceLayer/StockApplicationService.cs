using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.Stock;
using PharmacySystemInfrastructure.Stock;

namespace PharmacySystem.ApplicationServiceLayer
{
    public class StockApplicationService
    {
        private readonly SqlConnection connection;
        private StockRepository stockRepository;

        public StockApplicationService(StockRepository _stockRepository)
        {
            if (_stockRepository.GetConn() == null)
                throw new NotImplementedException();

            stockRepository = _stockRepository;
            connection = _stockRepository.GetConn();
        }

        public int GetInvIDByProIDandWareID(string proID, int wareID)
        {
            return stockRepository.GetInvIDByProIDandWareID(proID, wareID, connection);
        }

        public List<int> GetAllWareID()
        {
            return stockRepository.GetAllWareID(connection);
        }

        public List<InventoryRecord> GetInventoryRecord(int invID)
        {
            return stockRepository.GetStockRecord(invID, connection);
        }

        public bool SaveRecord(List<InventoryRecord> stockRecordList, EmployeeID employeeID)
        {
            foreach (var stockRecord in stockRecordList)
            {
                List<InventoryRecord> records = GetInventoryRecord(stockRecord.InvID); //取得所有紀錄
                List<InventoryRecord> currentInventory = StockCaculator.GetCurrentInventory(records); //計算目前庫存
                List<InventoryRecord> needInsertStock = StockCaculator.GetCanInsertInventoryRecord(stockRecord, currentInventory); //取得扣庫數量
                if (needInsertStock.Count == 0)
                    return false;

                foreach (var stock in needInsertStock)
                {
                    stock.ExecuteEmployeeID = employeeID;
                }
                stockRepository.SaveRecord(needInsertStock, connection);
            }

            return true;
        }
    }
}
