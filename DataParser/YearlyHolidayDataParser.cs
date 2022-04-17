using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Dapper;
using GeneralClass;

namespace DataParser
{
    public class YearlyHolidayDataParser
    {

        public static void UpdateData()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var url =
                "https://data.ntpc.gov.tw/api/datasets/308DCD75-6434-45BC-A95F-584DA4FED251/json?page=1&size=1000000";
            var request = WebRequest.Create(url);

            var response = request.GetResponse() as HttpWebResponse;

            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            var srcString = reader.ReadToEnd();
            var jsonData = Newtonsoft.Json.JsonConvert
                .DeserializeObject<Record[]>(srcString);
            var result = jsonData.Where(r => DateTime.Parse(r.date) >= DateTime.Today);
            foreach (var holiday in result)
            {
                Console.WriteLine(
                    $"Date: {holiday.date}, IsHoliday: {holiday.isHoliday}, Category: {holiday.holidayCategory}");

                 
                using (var conn = new SqlConnection(DBInvoker.ConnectionString))
                {
                    string sql = $"Insert into dbo.HolidayRecord (Holidate,HolidayName) Values('{holiday.date}','{holiday.holidayCategory}')";
                    conn.Execute(sql);
                }
            }

            Console.ReadKey();
           
        }

        public class HolidayOpenData
        {
            public bool success { get; set; }
            public Result result { get; set; }
        }

        public class Result
        {
            public string resource_id { get; set; }
            public int limit { get; set; }
            public int total { get; set; }
            public Field[] fields { get; set; }
            public Record[] records { get; set; }
        }

        public class Field
        {
            public string type { get; set; }
            public string id { get; set; }
        }

        public class Record
        {
            public string date { get; set; }
            public string name { get; set; }
            public string isHoliday { get; set; }
            public string holidayCategory { get; set; }
            public string description { get; set; }
        }

    }
}
