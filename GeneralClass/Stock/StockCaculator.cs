using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralClass.Stock
{
    public static class StockCaculator
    {
        //取得目前庫存
        public static List<InventoryRecord> GetCurrentInventory(List<InventoryRecord> dataHistory)
        {
            var inventory = new List<InventoryRecord>();

            foreach (var data in dataHistory)
            {
                if (data.Amount == 0)
                    throw new Exception("GetCurrentInventory() => Amount is 0");

                if (data.Amount > 0)
                    inventory.Add(data);
                else
                {
                    var currentDiff = data.Amount; //負的

                    while (currentDiff < 0)
                    {
                        var oldestData = inventory.First();
                        var minusValue = currentDiff * -1;

                        if (minusValue >= oldestData.Amount) //扣的超過此批庫存量
                        {
                            currentDiff += oldestData.Amount;
                            inventory.Remove(oldestData);
                        }
                        else
                        {
                            oldestData.Amount -= minusValue;
                            currentDiff = 0;
                        }
                    }
                }
            }

            return inventory;
        }

        //取得可扣庫資料
        public static List<InventoryRecord> GetCanInsertInventoryRecord(InventoryRecord insertInventory, List<InventoryRecord> currentInventory)
        {
            int invIDCount = currentInventory.GroupBy(_ => _.InvID).Distinct().Count();
            if (invIDCount > 1)
                throw new Exception("GetCanInsertInventoryRecord()  請勿傳入超過1種InvID的List<InventoryRecord>");

            List<dynamic> paramList = new List<dynamic>();
            List<InventoryRecord> needInsertStock = new List<InventoryRecord>();

            List<InventoryRecord> batchMatchInventory =
                currentInventory.Where(_ => _.BatchNumber == insertInventory.BatchNumber).ToList();

            if (insertInventory.Amount > 0)
            {  
                needInsertStock.Add(insertInventory);
            }
            else
            {
                var currentDiff = insertInventory.Amount * -1;

                if (batchMatchInventory.Sum(_ => _.Amount) < currentDiff)
                    return needInsertStock;//庫存不夠
                else
                {
                    int i = 0;
                    while (currentDiff > 0)
                    {
                        InventoryRecord inventoryRecord = new InventoryRecord();
                        inventoryRecord.Amount = 0; 
                        inventoryRecord.InvID = batchMatchInventory[i].InvID;
                        inventoryRecord.ExecuteTime = DateTime.Now;

                        if (currentDiff >= batchMatchInventory[i].Amount) //此批扣不夠或是剛剛好{
                        {
                            inventoryRecord.Amount = batchMatchInventory[i].Amount * -1;
                            inventoryRecord.TotalValue = batchMatchInventory[i].TotalValue * -1;
                        }
                        else
                        {
                            inventoryRecord.Amount = currentDiff * -1;
                            double singlePrice = (double)batchMatchInventory[i].TotalValue / (double)batchMatchInventory[i].Amount;
                            inventoryRecord.TotalValue = (int)(singlePrice * (double)currentDiff + 0.5) * -1;
                        }
                            
                        needInsertStock.Add(inventoryRecord);

                        currentDiff -= batchMatchInventory[i].Amount;
                        i++;
                    }
                }
            }
            //[xilian] 會將資料再加入currentInventory,for後續有多次新增的情況
            currentInventory.AddRange(needInsertStock);
            return needInsertStock;
        }

    }
}
