using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Service
{
    public class ControlMedicineDeclareService
    {

        public void Export(string filepath, IEnumerable<ControlMediceRecord> dataList)
        {

        }
    }

    public class ControlMediceRecord
    {
        public string HappenDate { get; set; } //日期

        public string Reason { get; set; } //收支原因

        public int GetAmount { get; set; } //收入數量

        public string BatchNumber { get; set; } //批號

        public int PayAmount { get; set; } //支出數量

        public int CurrentAmount { get; set; } //結存數量

        public string Remark { get; set; } //備註
    }
}
