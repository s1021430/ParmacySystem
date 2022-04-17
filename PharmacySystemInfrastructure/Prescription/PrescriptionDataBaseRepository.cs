using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Dapper;
using GeneralClass;
using GeneralClass.Customer.EntityIndex;
using GeneralClass.DeclareData;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalOrders;

namespace PharmacySystemInfrastructure.Prescription
{
    public class PrescriptionDataBaseRepository : IPrescriptionRepository
    {
        private readonly SqlConnection connection;

        private static readonly string[] PreMasColumns = 
        {
            "ID", "PatientID", "InstitutionID", "DivisionID", "AdjustCaseID",
            "PharmacistID", "MedicalSerialNum", "CopaymentID", "PrescriptionCaseID",
            "TreatDate", "AdjustDate", "MainDiseaseID", "SubDiseaseID", "PaymentCategoryID", "SpecialTreatID",
            "MedicinePoint", "SpecialMaterialPoint", "CopaymentPoint", "DeclarePoint", "TotalPoint",
            "MedicineSelfPayAmount"
        };

        private static readonly string[] PreDetailColumnList = 
        {
            "PreMasID", "Type", "ID", "Dosage", "Frequency", "ActionSite", "Days",
            "TotalAmount", "OwnExpense", "OwnExpensePrice", "TotalPoint",
            "AdditionRatio", "MedicalOrderSerialNumber", "ExecutionStart", "ExecutionEnd"
        };

        private const string PrescriptionRecordsQueryBase =
            @"SELECT P.ID AS ID, C.Name AS PatientName, C.Birthday AS PatientBirthday,C.IDNumber AS PatientIdNumber, 
            INS.Name AS Institution, P.DivisionID, P.AdjustCaseID, P.TreatDate,P.AdjustDate, P.MedicalSerialNum,
            EMP.Name AS Pharmacist FROM Prescription.Master AS P JOIN Customer.Master AS C ON P.PatientID = C.ID 
            JOIN NHI.Institution AS INS ON P.InstitutionID = INS.ID JOIN Employee.Master AS EMP ON P.PharmacistID = EMP.ID WHERE";

        public PrescriptionDataBaseRepository() { }

        public PrescriptionDataBaseRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void InsertPrescription(PrescriptionData pres)
        {
            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();
                pres.ID = GetPrescriptionLatestId(localConnection);
                InsertPreMas(pres, localConnection);
                InsertMedicalOrders(pres, localConnection);
                localConnection.Close();
            }
            else
            {
                pres.ID = GetPrescriptionLatestId(connection);
                InsertPreMas(pres, connection);
                InsertMedicalOrders(pres, connection);
            }
        }

        public void DeletePrescription(PrescriptionData pres)
        {
            var deleteSQL = $@"UPDATE Prescription.Master SET IsEnable = '0' WHERE ID = '{pres.ID.ID}'";
            if (connection is null)
            {
                using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                localConnection.Open();
                localConnection.Execute(deleteSQL);
                localConnection.Close();
            }
            else
                connection.Execute(deleteSQL);
        }

        public bool UpdatePrescription(PrescriptionData pres)
        {
            try
            {
                var updateSQL = $"UPDATE Prescription.Master SET {DBInvoker.GetUpdateTableColumns(PreMasColumns.Skip(1).ToArray())} WHERE ID = '{pres.ID.ID}'";
                if (connection is null)
                {
                    using var localConnection = new SqlConnection(DBInvoker.ConnectionString);
                    localConnection.Open();
                    localConnection.Execute(updateSQL);
                    localConnection.Close();
                }
                else
                    connection.Execute(updateSQL);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<PrescriptionRecord> GetPrescriptionRecordsBySearchPattern(PrescriptionSearchPattern searchPattern)
        {
            var sql = PrescriptionRecordsQueryBase;
            sql += GetSearchCondition();
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            return conn.Query<PrescriptionRecord>(sql).ToList();

            string GetSearchCondition()
            {
                var searchSqlCondition = string.Empty;
                searchSqlCondition += @$" P.AdjustDate >= '{searchPattern.StartDate:yyyy-MM-dd}' ";
                searchSqlCondition += @$" AND P.AdjustDate <= '{searchPattern.EndDate:yyyy-MM-dd}' ";
                if (!string.IsNullOrEmpty(searchPattern.PatientIdNumber))
                    searchSqlCondition += $"AND C.IDNumber LIKE '%{searchPattern.PatientIdNumber}%'";
                if (!string.IsNullOrEmpty(searchPattern.PatientName))
                    searchSqlCondition += $" AND C.Name LIKE '%{searchPattern.PatientName}%'";
                if (!string.IsNullOrEmpty(searchPattern.InstitutionId))
                    searchSqlCondition += $@" AND P.InstitutionID = '{searchPattern.InstitutionId}' ";
                if (!string.IsNullOrEmpty(searchPattern.AdjustCaseId))
                    searchSqlCondition += $@" AND  P.AdjustCaseID = '{searchPattern.AdjustCaseId}' ";
                return searchSqlCondition;
            }
        }

        private PrescriptionID GetPrescriptionLatestId(SqlConnection localConnection)
        {
            const string queryLatestID = @"SELECT ISNULL(Max(ID),0) + 1 FROM Prescription.Master";
            var result = localConnection.QueryFirst<int>(queryLatestID);
            return new PrescriptionID(result);
        }

        private bool InsertPreMas(PrescriptionData data,SqlConnection localConnection)
        {
            var insertPrescriptionSQL = $@"INSERT INTO Prescription.Master ({DBInvoker.GetTableColumns(PreMasColumns)}) VALUES({DBInvoker.GetTableParameterColumns(PreMasColumns)});";
            try
            {
                localConnection.Execute(insertPrescriptionSQL, GetPrescriptionDAO(data));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool InsertMedicalOrders(PrescriptionData data, SqlConnection localConnection)
        {
            if (data.MedicalOrders == null || data.MedicalOrders.Count == 0)
                return false;
            data.MedicalOrders.ForEach(order => order.PreMasID = data.ID);
            var sql = $@"INSERT INTO Prescription.Detail ({DBInvoker.GetTableColumns(PreDetailColumnList)}) VALUES({DBInvoker.GetTableParameterColumns(PreDetailColumnList)});";
            try
            {
                localConnection.Execute(sql, data.MedicalOrders);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<PrescriptionData> GetDeclarePrescriptionsOverview(int year, int month)
        {
            var sql = "SELECT * FROM Prescription.Master " +
                      $"WHERE DATEPART(YEAR,AdjustDate) = {year} AND DATEPART(MONTH, AdjustDate) = '{month}'";
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            var result = conn.Query<PrescriptionData>(sql).ToList();
            return result;
        }

        public void SaveDeclareFile(DeclareFile declareFile, string path, string declareFileName)
        {
            path += "\\DRUGT.xml";
            var xmlSerializer = new XmlSerializer(declareFile.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, declareFile);
                var document = XDocument.Parse(XmlExtension.PrettyXml(textWriter));
                var root = XElement.Parse(document.ToString());
                document = XDocument.Load(root.CreateReader());
                document.Root?.RemoveAttributes();
                document.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
                document.Declaration = new XDeclaration("1.0", "Big5", string.Empty);
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.GetEncoding(950),
                    OmitXmlDeclaration = false
                };
                var writer = XmlWriter.Create(path, settings);
                document.Save(writer);
                writer.Close();
                var info = File.ReadAllText(path, Encoding.GetEncoding(950));
                File.WriteAllText(path, info.Replace("big5", "Big5"), Encoding.GetEncoding(950));
            }

            var targetDirectory = Path.Combine(Path.GetDirectoryName(path), declareFileName);
            FileExtension.CompressToZip(new List<string> { path }, targetDirectory);
        }

        public void ImportPrescriptionFromDeclareFiles(string declareDirectory)
        {
            var files = Directory.GetFiles(declareDirectory, "*.xml");
            var result = ParallelDeserialize(files);
        }

        private static IEnumerable<DeclareFile> ParallelDeserialize(IEnumerable<string> files)
        {
            var serializer = new XmlSerializer(typeof(DeclareFile));
            Task xmlLoadingTask = null;
            using var declareFileDocuments = new BlockingCollection<XDocument>();
            try
            {
                xmlLoadingTask = new Task(() =>
                {
                    foreach (var file in files)
                        declareFileDocuments.CompleteAdding();
                });
                xmlLoadingTask.Start();
                foreach (var e in declareFileDocuments.GetConsumingEnumerable())
                    yield return (DeclareFile)serializer.Deserialize(e.CreateReader());
            }
            finally
            {
                if (xmlLoadingTask != null)
                {
                    Task.WaitAll(xmlLoadingTask);
                    xmlLoadingTask.Dispose();
                }
                declareFileDocuments.Dispose();
            }
        }

        public List<PrescriptionData> GetPrescriptionsById(IEnumerable<PrescriptionID> prescriptionsId)
        {
            var sql = "SELECT PS_M.*, PS_D.* " +
                      "FROM Prescription.Master AS PS_M LEFT JOIN Prescription.Detail AS PS_D ON PS_M.ID = PS_D.PreMasID " +
                      $"WHERE PS_M.ID IN ({string.Join(",", prescriptionsId.Select(_ => _.ID))})";
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            var prescriptionDictionary = new Dictionary<PrescriptionID, PrescriptionData>();
            var result = conn.Query<PrescriptionData, MedicalOrder, PrescriptionData>(
                sql,
                (prescription, medicalOrder) =>
                {
                    if (!prescriptionDictionary.TryGetValue(prescription.ID, out var prescriptionEntry))
                    {
                        prescriptionEntry = prescription;
                        prescriptionDictionary.Add(prescriptionEntry.ID, prescriptionEntry);
                    }

                    prescriptionEntry.MedicalOrders.Add(medicalOrder);
                    return prescriptionEntry;
                },
                splitOn: "PreMasID").Distinct().ToList();
            return result;
        }

        public List<PrescriptionData> GetPrescritionsByMedicalOrderIDAndDate(IEnumerable<MedicalOrderID> orderID, DateTime startDate, DateTime endDate)
        {
            var sql = $@"Select preMaster.*,preDetail.* 
            from Prescription.Master preMaster left join Prescription.Detail preDetail on preMaster.ID = preDetail.PreMasID
            where 
            AdjustDate >= '{startDate:yyyy-MM-dd}' and AdjustDate <= '{endDate:yyyy-MM-dd}' and
            preDetail.ID in ({string.Join(",", orderID.Select(_ => $"'{_.ID}'" ))})";

            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            var prescriptionDictionary = new Dictionary<PrescriptionID, PrescriptionData>();

            var result = conn.Query<PrescriptionData, MedicalOrder, PrescriptionData>(
                sql,
                (prescription, medicalOrder) =>
                {
                    if (!prescriptionDictionary.TryGetValue(prescription.ID, out var prescriptionEntry))
                    {
                        prescriptionEntry = prescription;
                        prescriptionDictionary.Add(prescriptionEntry.ID, prescriptionEntry);
                    }

                    prescriptionEntry.MedicalOrders.Add(medicalOrder);
                    return prescriptionEntry;
                },
                splitOn: "PreMasID").Distinct().ToList();
            return result;

        }

        private object GetPrescriptionDAO(PrescriptionData prescription)
        {
            return new
            {
                prescription.ID.ID,
                InstitutionID = prescription.InstitutionID.ID,
                DivisionID = prescription.DivisionID.ID,
                AdjustCaseID = prescription.AdjustCaseID.ID,
                PharmacistID = prescription.Pharmacist != null ? prescription.Pharmacist.ID.ID.ToString() : string.Empty,
                CopaymentID = prescription.CopaymentID.ID,
                PrescriptionCaseID = prescription.PrescriptionCaseID.ID,
                PaymentCategoryID = prescription.PaymentCategoryID.ID,
                SpecialTreatID = prescription.SpecialTreatID.ID,
                PatientID = prescription.Patient is not null ? prescription.Patient.ID.ID : new CustomerID(-1).ID,
                MainDiseaseID = prescription.MainDiseaseID.ID,
                SubDiseaseID = prescription.SubDiseaseID.ID,
                prescription.MedicalSerialNum,
                prescription.TreatDate,
                prescription.AdjustDate,
                prescription.MedicinePoint,
                prescription.SpecialMaterialPoint,
                prescription.CopaymentPoint,
                prescription.MedicalServicePoint,
                prescription.TotalPoint,
                prescription.DeclarePoint,
                prescription.MedicineSelfPayAmount,
                prescription.MedicineDays,
            };
        }
    }
}