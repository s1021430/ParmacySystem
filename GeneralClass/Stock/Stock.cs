using System;
using GeneralClass.Employee.EntityIndex;

namespace GeneralClass.Stock
{ 
    public class InventoryRecord  
    { 
        public InventoryRecord(){}
        public StockType Type { get; set; }

        public int InvID { get; set; } //庫存碼

        public int Amount { get; set; } //數量

        public int TotalValue { get; set; } //總金額

        public string BatchNumber { get; set; } //批號

        public string SourceID { get; set; } //來源ID (處方,訂單....)

        public DateTime ExecuteTime { get; set; }

        public EmployeeID ExecuteEmployeeID { get; set; } 
    }


    public enum StockType
    {
        Prescription = 0, //調劑
        Purchase = 1, //進貨
        Return = 2 //退貨
    }
}
