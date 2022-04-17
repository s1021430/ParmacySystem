using GeneralClass.Employee.EntityIndex;

namespace GeneralClass.Stock
{
    public class InventoryRecordFactory
    {
        public static InventoryRecord CreateInventoryRecord(
            int invID,
            string batchNumber,
            int amount,
            StockType type,
            string sourceID,
            EmployeeID empID)
        { 
            if (invID == -1)
                return null;

            InventoryRecord result = new InventoryRecord()
            {
                Type = type,
                InvID = invID,
                Amount = amount,
                TotalValue = 0,
                BatchNumber = batchNumber,
                SourceID = sourceID,
                ExecuteEmployeeID = empID
            };
            return result;
        }
    }
}
