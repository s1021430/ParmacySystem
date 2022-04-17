using System;
using System.Collections.Generic;
using System.Linq;
using GeneralClass.Stock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystemTest.Stock
{
    [TestClass]
    class StockApplicationServiceUnitTests
    {
        private StockApplicationService stockService;

        private TestContext testContextInstance;
        
        [TestMethod]
        public void Test_Exception_Amount0()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = 0;
            inventoryRecords.Add(record);

            try
            {
                List<InventoryRecord> result = StockCaculator.GetCurrentInventory(inventoryRecords);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);

        }

        [TestMethod]
        public void Test_單品項單次新增_庫存計算()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            record.TotalValue = 5;
            inventoryRecords.Add(record);
            List<InventoryRecord> result = StockCaculator.GetCurrentInventory(inventoryRecords);

            Assert.AreEqual(inventoryRecords.Sum(_ => _.Amount), result.Sum(_ => _.Amount));
            Assert.AreEqual(inventoryRecords.Sum(_ => _.TotalValue), result.Sum(_ => _.TotalValue));
        }

        [TestMethod]
        public void Test_單品項多次新增_庫存計算()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                var record = Substitute.For<InventoryRecord>();
                record.Amount = 10;
                record.TotalValue = 50;
                inventoryRecords.Add(record);
            }
            List<InventoryRecord> result = StockCaculator.GetCurrentInventory(inventoryRecords);

            Assert.AreEqual(inventoryRecords.Sum(_ => _.Amount), result.Sum(_ => _.Amount));
            Assert.AreEqual(inventoryRecords.Sum(_ => _.TotalValue), result.Sum(_ => _.TotalValue));
        }

        [TestMethod]
        public void Test_多品項單次新增_庫存計算()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();

            var record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            record.TotalValue = 5;
            record.InvID = 1;
            inventoryRecords.Add(record);

            record = Substitute.For<InventoryRecord>();
            record.Amount = 2;
            record.TotalValue = 7;
            record.InvID = 2;
            inventoryRecords.Add(record);
            List<InventoryRecord> result = StockCaculator.GetCurrentInventory(inventoryRecords);

            Assert.AreEqual(
                inventoryRecords.Where(_ => _.InvID == 0).Sum(_ => _.Amount),
                result.Where(_ => _.InvID == 0).Sum(_ => _.Amount));

            Assert.AreEqual(inventoryRecords.Where(_ => _.InvID == 0).Sum(_ => _.TotalValue),
                result.Where(_ => _.InvID == 0).Sum(_ => _.TotalValue));

            Assert.AreEqual(
                inventoryRecords.Where(_ => _.InvID == 1).Sum(_ => _.Amount),
                result.Where(_ => _.InvID == 1).Sum(_ => _.Amount));

            Assert.AreEqual(inventoryRecords.Where(_ => _.InvID == 1).Sum(_ => _.TotalValue),
                result.Where(_ => _.InvID == 1).Sum(_ => _.TotalValue));
        }

        [TestMethod]
        public void Test_單品項庫存0直接扣庫()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = -1;
            List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(record, inventoryRecords);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_單品項無批號扣庫剛好扣完()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            inventoryRecords.Add(record);

            var bucklerecord = Substitute.For<InventoryRecord>();
            bucklerecord.Amount = -1;
            List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(bucklerecord, inventoryRecords);

            Assert.AreEqual(bucklerecord.Amount, result.Sum(_ => _.Amount));
        }

        [TestMethod]
        public void Test_單品項有批號_單次新增()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            record.TotalValue = 20;
            record.BatchNumber = "AA";

            List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(record, inventoryRecords);

            Assert.AreEqual(record.Amount, result.Sum(_ => _.Amount));
            Assert.AreEqual(record.TotalValue, result.Sum(_ => _.TotalValue));
            Assert.AreEqual(record.BatchNumber, result.First().BatchNumber);
        }

        [TestMethod]
        public void Test_單品項不同批號_多次新增()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var recordA = Substitute.For<InventoryRecord>();
            recordA.Amount = 1;
            recordA.TotalValue = 20;
            recordA.BatchNumber = "AA";

            var recordB = Substitute.For<InventoryRecord>();
            recordB.Amount = 2;
            recordB.TotalValue = 50;
            recordB.BatchNumber = "BB";
            List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(recordA, inventoryRecords);

            Assert.AreEqual(recordA.Amount, result.Sum(_ => _.Amount));
            Assert.AreEqual(recordA.TotalValue, result.Sum(_ => _.TotalValue));
            Assert.AreEqual(recordA.BatchNumber, result.First().BatchNumber);

            result = StockCaculator.GetCanInsertInventoryRecord(recordB, result);

            Assert.AreEqual(recordB.Amount, result.Sum(_ => _.Amount));
            Assert.AreEqual(recordB.TotalValue, result.Sum(_ => _.TotalValue));
            Assert.AreEqual(1, result.Count(_ => _.BatchNumber == recordB.BatchNumber));
        }

        [TestMethod]
        public void Test_多品項無批號_多次新增刪除()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            int productCategoryCount = 10000;
            for (int i = 1; i < productCategoryCount; i++)
            {
                var record = Substitute.For<InventoryRecord>();
                record.InvID = i;
                record.Amount = i;
                record.TotalValue = i;
                inventoryRecords.Add(record);
            }

            for (int i = 1; i < productCategoryCount; i++)
            {
                var record = Substitute.For<InventoryRecord>();
                record.InvID = i;
                record.Amount = i * -1;

                List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(record, inventoryRecords.Where(_ => _.InvID == i).ToList());

                Assert.AreEqual(record.InvID, result.First().InvID);
                Assert.AreEqual(record.Amount, result.Sum(_ => _.Amount));
                Assert.AreEqual(i * -1, result.Sum(_ => _.TotalValue));
            }


        }

        [TestMethod]
        public void Test_單品項多批號_扣庫無庫存的批號()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            record.TotalValue = 1;
            record.BatchNumber = "A";
            inventoryRecords.Add(record);

            record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            record.TotalValue = 1;
            record.BatchNumber = "B";
            inventoryRecords.Add(record);

            record = Substitute.For<InventoryRecord>();
            record.Amount = 1;
            record.TotalValue = 1;
            record.BatchNumber = "C";
            inventoryRecords.Add(record);

            record = Substitute.For<InventoryRecord>();
            record.Amount = -1;
            record.BatchNumber = "D";
            List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(record, inventoryRecords);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_Exception_傳入InvID大於1種的ListRecord()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.InvID = 0;
            inventoryRecords.Add(record);

            record = Substitute.For<InventoryRecord>();
            record.InvID = 1;
            inventoryRecords.Add(record);
            try
            {
                List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(record, inventoryRecords);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_單品項多批號_扣庫()
        {
            List<InventoryRecord> inventoryRecords = new List<InventoryRecord>();
            var record = Substitute.For<InventoryRecord>();
            record.Amount = 20;
            record.TotalValue = 20;
            record.BatchNumber = "A";
            inventoryRecords.Add(record);

            record = Substitute.For<InventoryRecord>();
            record.Amount = 30;
            record.TotalValue = 60;
            record.BatchNumber = "B";
            inventoryRecords.Add(record);

            var buckleRecord = Substitute.For<InventoryRecord>();
            buckleRecord.Amount = -15;
            buckleRecord.BatchNumber = "A";

            List<InventoryRecord> result = StockCaculator.GetCanInsertInventoryRecord(buckleRecord, inventoryRecords);

            Assert.AreEqual(-15, result.Sum(_ => _.TotalValue));
        }
    }
}
