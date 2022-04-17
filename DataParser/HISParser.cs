using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using GeneralClass;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using ExcelDataReader;
using Dapper;

namespace DataParser
{

    public class HISParser
    {
        private delegate void ReadFileStringArrayCallBack(string strArray);

        private static readonly string FolderPath = Path.Combine("C:\\PharmacySystem", "DownloadData"); 

        public HISParser()
        {
            
        }

        //第一次安裝用這個
        public void FirstTimeDBInitial()
        {
            CreateBaseData();
            UpdateLastestMedicineData();
            UpdateLatestPharmacyData();
            UpdateLatestClinicData();
            UpdateLatestDiseaseCodeData();
            UpdateLatestSpecialMaterial();
        }

        private void CreateBaseData()
        {
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();

            //Add Default WareHouse
            conn.Execute(@" Insert into Product.WareHouse
                        (ID, Name, IsEnable)  Values(0, '藥局', 1);");

            //Add Default ProductType
            conn.Execute(@"Insert Into Product.Type
                        (ID,Name,Level,Parent_ID,IsEnable) Values(0,'Medicine',0,0,1);");

            conn.Close();
        }

        public void UpdateLastestMedicineData()
        {
            var remoteUri = "https://data.nhi.gov.tw/Datasets/DatasetResource.ashx?rId=A21030000I-E41001-001";
            var filePath = Path.Combine(FolderPath, "MedicineData.txt");
            DownLoadFromURI(remoteUri, filePath);
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var paramList = new List<dynamic>();
           
            var culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            GetFileDataObject(filePath, ((datastring) =>
            {
                string[] data;
                if (datastring.Split(',').Length == 15)
                {
                    data = datastring.Split(',');
                    data[3] = data[3] + data[4];
                    for (var i = 4; i < 14; i++)
                    {
                        data[i] = data[i + 1];
                    }
                }
                else if (datastring.Split(',').Length == 16)
                {
                    data = datastring.Split(',');
                    data[3] = data[3] + data[4] + data[5];
                    for (var i = 4; i < 14; i++)
                    {
                        data[i] = data[i + 2];
                    }
                }
                else
                {
                    data = datastring.Split(',');
                }

                var isChange = data[0] != string.Empty; //異動 
                var productID = data[1]; //藥品代號
                var engName = textInfo.ToTitleCase(data[2]); //藥品英文名稱
                var chineseName = data[3]; //藥品中文名稱
                double.TryParse(data[4], out var amount);
                var unit = data[5]; //規格單位
                var isSingle = data[6] == "單方"; //單複方
                var price = data[7]; //參考價
                var startDate =
                    $"{data[8].Substring(0, 3)}/{data[8].Substring(3, 2)}/{data[8].Substring(5, 2)}"; //有效起日
                var endDate =
                    $"{data[9].Substring(0, 3)}/{data[9].Substring(3, 2)}/{data[9].Substring(5, 2)}"; //有效迄日
                var manufactory = data[10]; //製造廠名稱
                var form = data[11]; //劑型
                var ingredient = textInfo.ToTitleCase(data[12]); //成份
                var atcCode = data[13]; //ATC_CODE

                var sDate = DateTime.Parse(startDate, culture);
                var eDate = DateTime.Parse(endDate, culture);
                paramList.Add(new
                {
                    ID = productID,
                    ChineseName = chineseName,
                    EnglishName = engName,
                    Amount = amount,
                    Unit = unit,
                    Manufactory = manufactory,
                    Form = form,
                    IsSingle = isSingle,
                    Ingredient = ingredient,
                    ATC = atcCode.Trim().PadLeft(8),
                    Price = price,
                    StartDate = sDate,
                    EndDate = eDate
                });

            }));
            var distParamList = paramList.Select(_ => new
            {
                _.ID,
                _.ChineseName,
                _.EnglishName,
                _.Amount,
                _.Unit,
                _.Manufactory,
                _.Form,
                _.IsSingle,
                _.Ingredient,
                _.ATC
            }).Distinct().ToList();

            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            //清空Table
            conn.Execute("Delete NHI.MedicinePrice", null);
            conn.Execute("Delete NHI.Medicine", null);

            //塞入NHI.Medicine
            conn.Execute(@"insert NHI.Medicine
                    (ID, ChineseName,EnglishName,Amount,Unit,Manufactory,Form,IsSingle,Ingredient,ATC) 
                values (@ID, @ChineseName,@EnglishName,@Amount,@Unit,@Manufactory,@Form,@IsSingle,@Ingredient,@ATC)", distParamList);

            //塞入NHI.MedicinePrice
            conn.Execute(@"insert NHI.MedicinePrice
                   (ID, Price,StartDate,EndDate) 
               values (@ID, @Price,@StartDate,@EndDate)", paramList);

            //取出目前最大INVID
            var startINVID = conn.QueryFirstOrDefault<int>(@"Select IsNull(Max(INV_ID),0)+1 MaxINVID from Product.InventoryMapping;");
            var needInsertMedTable = new DataTable();

            //找出需要新增的藥品
            needInsertMedTable.Load(conn.ExecuteReader(@" Select ID, ChineseName, EnglishName,0 TYPE_ID,'' as Barcode,'' as Note,'true' as IsEnable
                    from NHI.Medicine
                    where ID not in (Select ID from Product.Master);"));
                 
            var needInsertMedList = new List<dynamic>();
            foreach (DataRow row in needInsertMedTable.Rows)
            {

                needInsertMedList.Add(new
                {
                    ID = row.Field<string>("ID"),
                    ChineseName = row.Field<string>("ChineseName"),
                    EnglishName = row.Field<string>("EnglishName"),
                    TYPE_ID = row.Field<int>("TYPE_ID"),
                    Barcode = row.Field<string>("Barcode"),
                    Note = row.Field<string>("Note"),
                    IsEnable = row.Field<string>("IsEnable"),
                    InvID = startINVID,
                    SafeAmount = 0,
                    BasicAmount = 0,
                    WareID = 0
                });
                startINVID++;
            }
            //塞入Product.Master
            conn.Execute(@"Insert into Product.Master
                    (ID, ChineseName, EnglishName, TYPE_ID, Barcode, Note, IsEnable)
                    values (@ID, @ChineseName, @EnglishName, @TYPE_ID, @Barcode, @Note, @IsEnable)", needInsertMedList);

            //塞入Product.Inventory
            conn.Execute(@"Insert into Product.Inventory
                    (ID,SafeAmount,BasicAmount)
                    values(@InvID, @SafeAmount, @BasicAmount)", needInsertMedList);

            //塞入InventoryMapping
            conn.Execute(@" Insert into Product.InventoryMapping
                    (PRO_ID, WARE_ID, INV_ID)
                    values(@ID, @WareID, @InvID)", needInsertMedList);

            conn.Close();
        }

        public void UpdateLatestPharmacyData()
        {
            var remoteUri = "https://data.nhi.gov.tw/DataSets/DataSetResource.ashx?rId=A21030000I-D21005-001";
            var filePath = Path.Combine(FolderPath, "PharmacyData.txt");
            DownLoadFromURI(remoteUri, filePath);

            var paramList = new List<dynamic>();
            GetFileDataObject(filePath, ((data) =>
            {
                var pharmacyData = data.Split(',');

                var id = pharmacyData[0]; //醫事機構代碼
                var name = pharmacyData[1]; //醫事機構名稱
                var type = pharmacyData[2]; //醫事機構種類
                var phone = pharmacyData[3]; //電話
                var address = pharmacyData[4]; //地址
                var area = pharmacyData[5]; //分區業務組
                var pharmacyType = pharmacyData[6]; //特約類別
                var serviceList = pharmacyData[7]; //服務項目
                var division = pharmacyData[8]; //診療科別
                var endTime = DateTime.ParseExact(pharmacyData[9],"yyyyMMdd", null); //終止合約或歇業日期
                var businessHours = pharmacyData[10];//固定看診時段
                var remark = pharmacyData[11]; //備註
                var cityCode = pharmacyData[12]; //縣市別代碼 

                paramList.Add(new
                {
                    ID = id,
                    Name = name,
                    Type = type,
                    Phone = phone,
                    Address = address,
                    Area = area,
                    PharmacyType = pharmacyType,
                    ServiceList = serviceList,
                    Devision = division,
                    EndTime = endTime,
                    BusinessHours = businessHours,
                    Remark = remark,
                    CityCode = cityCode
                });
            }));
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Execute("Delete NHI.Pharmacy");
            conn.Execute(@"Insert Into NHI.Pharmacy
                (ID,Name,Type,Phone,Address,Area,PharmacyType,ServiceList,Devision,EndTime,BusinessHours,Remark,CityCode)
                Values(@ID,@Name,@Type,@Phone,@Address,@Area,@PharmacyType,@ServiceList,@Devision,@EndTime,@BusinessHours,@Remark,@CityCode)", paramList);
        }
        public void UpdateLatestClinicData() //Total Clinic Data
        {
            var remoteUri = "https://data.nhi.gov.tw/Datasets/DatasetResource.ashx?rId=A21030000I-D21003-004"; //地區醫院
            var filePath = Path.Combine(FolderPath, "AreaHospitalData.txt");
            DownLoadFromURI(remoteUri, filePath);
            var paramList = new List<dynamic>();
            GetFileDataObject(filePath, ((data) =>
            {
                var hospitalData = data.Split(',');

                var id = hospitalData[0];//醫事機構代碼
                var name = hospitalData[1];//醫事機構名稱 
                var phone = hospitalData[3];//電話 
                var address = hospitalData[4];//地址 
                var endTime = DateTime.ParseExact(hospitalData[9],"yyyyMMdd",null); //終止合約或歇業日期 

                paramList.Add(new
                {
                    ID = id,
                    Name = name,
                    Address = address,
                    Telephone = phone,
                    EndContractDate = endTime
                });
            }));

            remoteUri = "https://data.nhi.gov.tw/DataSets/DataSetResource.ashx?rId=A21030000I-D21004-008"; //診所
            filePath = Path.Combine(FolderPath, "ClinicData.txt");
            DownLoadFromURI(remoteUri, filePath);
            GetFileDataObject(filePath, ((data) =>
            {
                var hospitalData = data.Split(',');

                var id = hospitalData[0];//醫事機構代碼
                var name = hospitalData[1];//醫事機構名稱
                var phone = hospitalData[3];//電話
                var address = hospitalData[4];//地址
                var endTime = DateTime.ParseExact(hospitalData[9], "yyyyMMdd", null); //終止合約或歇業日期

                paramList.Add(new
                {
                    ID = id,
                    Name = name,
                    Address = address,
                    Telephone = phone,
                    EndContractDate = endTime
                });

            }));

            remoteUri = "https://data.nhi.gov.tw/Datasets/DatasetResource.ashx?rId=A21030000I-D21001-003"; //醫學中心
            filePath = Path.Combine(FolderPath, "HISCenterData.txt");
            DownLoadFromURI(remoteUri, filePath);
            GetFileDataObject(filePath, ((data) =>
            {
                var hospitalData = data.Split(',');

                var id = hospitalData[0];//醫事機構代碼
                var name = hospitalData[1];//醫事機構名稱
                var phone = hospitalData[3];//電話
                var address = hospitalData[4];//地址
                var endTime = DateTime.ParseExact(hospitalData[9], "yyyyMMdd", null); //終止合約或歇業日期

                paramList.Add(new
                {
                    ID = id,
                    Name = name,
                    Address = address,
                    Telephone = phone,
                    EndContractDate = endTime
                });

            }));

            remoteUri = "https://data.nhi.gov.tw/DataSets/DataSetResource.ashx?rId=A21030000I-D21002-005"; //區域醫院
            filePath = Path.Combine(FolderPath, "RegionHospitalData.txt");
            DownLoadFromURI(remoteUri, filePath);
            GetFileDataObject(filePath, ((data) =>
             {
                 var hospitalData = data.Split(',');

                 var id = hospitalData[0];//醫事機構代碼
                 var name = hospitalData[1];//醫事機構名稱
                 var phone = hospitalData[3];//電話
                 var address = hospitalData[4];//地址
                 var endTime = DateTime.ParseExact(hospitalData[9], "yyyyMMdd", null); //終止合約或歇業日期

                 paramList.Add(new
                 {
                     ID = id,
                     Name = name,
                     Address = address,
                     Telephone = phone,
                     EndContractDate = endTime
                 });

             }));
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Execute("Delete NHI.Institution;");
            conn.Execute(@"Insert into NHI.Institution
                            (ID,Name,Address,Telephone,EndContractDate)
                            Values(@ID,@Name,@Address,@Telephone,@EndContractDate)", paramList.Distinct());
        }

        public void UpdateLatestDiseaseCodeData()
        {
            //10-PCS
            var remoteUri = "https://ws.nhi.gov.tw/Download.ashx?u=LzAwMS9VcGxvYWQvMjkyL3JlbGZpbGUvMC8yNDUzOC8xLjIg5Lit5paH54mIaWNkLTEwLXBjcygxMDYuMDcuMTnmm7TmlrApLnhsc3g%3d&n=MS4yIOS4reaWh%2beJiElDRC0xMC1QQ1MoMTA2LjA3LjE55pu05pawKS54bHN4";
            var filePath = Path.Combine(FolderPath, "DiseaseCode_PCS10.xlsx");
            DownLoadFromURI(remoteUri, filePath);
            var paramList = new List<dynamic>();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                using (reader)
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        { 
                            UseHeaderRow = false  //設定讀取資料時是否忽略標題
                        }
                    }); 
                    foreach (DataTable table in ds.Tables)
                    { 
                        if(table.TableName == "修正說明")
                            continue;

                        for (var row = 2; row < table.Rows.Count; row+=2)
                        { 
                            paramList.Add(new
                            {
                                ID = table.Rows[row][0].ToString(),
                                EnglishName = table.Rows[row][1].ToString(),
                                ChineseName = table.Rows[row + 1][1].ToString(),
                            });  
                        }
                    } 
                }
            }

            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Execute(@"Delete NHI.DiseaseCode");
            conn.Execute(@"Insert Into NHI.DiseaseCode
                            (ID,ChineseName,EnglishName,Type)
                            Values(@ID,@ChineseName,@EnglishName,'10')", paramList.Distinct());
        }

        public void UpdateLatestSpecialMaterial()
        {
            var remoteUri = @"https://www.nhi.gov.tw/DL.aspx?sitessn=292&u=LzAwMS9VcGxvYWQvMjkyL3JlbGZpbGUvMC83MjAwL3RvdGFsXzExMDAyLmNzdg%3d%3d&n=dG90YWxfMTEwMDIuY3N2&ico%20=.csv";
            var filePath = Path.Combine(FolderPath, "SpecialMaterial.txt");
            //DownLoadFromURI(remoteUri, filePath); 
            var paramList = new List<dynamic>();
            var culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            string line;
            var file = new StreamReader(filePath, Encoding.Default);
            file.ReadLine();
            var header = file.ReadLine();
            while ((line = file.ReadLine()) != null)
            {
                var lineData = line.Split(',');

                paramList.Add(new
                {
                    ID = lineData[7], 
                    ChineseName = lineData[9],
                    EnglishName = lineData[10],
                    Unit = lineData[12],
                    Manufactory = lineData[20],
                    Document = lineData[21],
                    Price = lineData[13],
                    StartDate = DateTime.Parse(lineData[15], culture), 
                    EndDate = DateTime.Parse(lineData[19], culture)
                });
            }
            file.Close();
            
            var distParamList = paramList.Select(_ => new
            {
                _.ID,
                _.ChineseName,
                _.EnglishName, 
                _.Unit,
                _.Manufactory,
                _.Document
            }).Distinct().ToList();

            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            conn.Execute(@"Delete NHI.SpecialMedicinePrice;");
            conn.Execute(@"Delete NHI.SpecialMedicine;");

            conn.Execute(@"Insert Into NHI.SpecialMedicine
                (ID, ChineseName, EnglishName, Unit, Manufactory, DocumentID)
            Values(@ID, @ChineseName, @EnglishName, @Unit, @Manufactory, @Document);", distParamList);

            conn.Execute(@" Insert Into NHI.SpecialMedicinePrice
                (SPEMED_ID, Price, StartDate, EndDate)
                Values(@ID,@Price,@StartDate,@EndDate)", paramList);

            var startINVID = conn.QueryFirstOrDefault<int>(@"Select IsNull(Max(INV_ID),0)+1 MaxINVID from Product.InventoryMapping;");

            //找出需要新增的藥品
            var needInsertMedTable = new DataTable();
                 
            needInsertMedTable.Load(conn.ExecuteReader(@" Select ID, ChineseName, EnglishName,0 TYPE_ID,'' as Barcode,'' as Note,'true' as IsEnable
                    from NHI.SpecialMedicine
                    where ID not in (Select ID from Product.Master);"));
                 
            var needInsertMedList = new List<dynamic>();
            foreach (DataRow row in needInsertMedTable.Rows)
            {

                needInsertMedList.Add(new
                {
                    ID = row.Field<string>("ID"),
                    ChineseName = row.Field<string>("ChineseName"),
                    EnglishName = row.Field<string>("EnglishName"),
                    TYPE_ID = row.Field<int>("TYPE_ID"),
                    Barcode = row.Field<string>("Barcode"),
                    Note = row.Field<string>("Note"),
                    IsEnable = row.Field<string>("IsEnable"),
                    InvID = startINVID,
                    SafeAmount = 0,
                    BasicAmount = 0,
                    WareID = 0
                });
                startINVID++;
            }
            //塞入Product.Master
            conn.Execute(@"Insert into Product.Master
                    (ID, ChineseName, EnglishName, TYPE_ID, Barcode, Note, IsEnable)
                    values (@ID, @ChineseName, @EnglishName, @TYPE_ID, @Barcode, @Note, @IsEnable)", needInsertMedList);

            //塞入Product.Inventory
            conn.Execute(@"Insert into Product.Inventory
                    (ID,SafeAmount,BasicAmount)
                    values(@InvID, @SafeAmount, @BasicAmount)", needInsertMedList);

            //塞入InventoryMapping
            conn.Execute(@" Insert into Product.InventoryMapping
                    (PRO_ID, WARE_ID, INV_ID)
                    values(@ID, @WareID, @InvID)", needInsertMedList);

            conn.Close();
        }

        private void GetFileDataObject(string filePath, ReadFileStringArrayCallBack arrayCallBack)
        {
            string line;
            var file = new StreamReader(filePath);
            file.ReadLine();//header
            while ((line = file.ReadLine()) != null)
            {
                arrayCallBack(line);
            }
            file.Close();
        }

        private void DownLoadFromURI(string remoteURI, string filePath)
        {
            Directory.CreateDirectory(FolderPath);
            // Create a new WebClient instance.
            using WebClient myWebClient = new();
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(remoteURI, filePath);
        }
    }
}
