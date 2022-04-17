using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnector
{
    public struct SQLInfo
    {
        public SQLInfo(string strSQL, Dictionary<string, object> sqlParam)
        {
            StrSQL = strSQL;
            SQLParam = sqlParam;
        }
        public string StrSQL { get; set; }
        public Dictionary<string, object> SQLParam { get; set; }
    }
}
