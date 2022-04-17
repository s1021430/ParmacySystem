using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GeneralClass;
using GeneralClass.Customer;
using GeneralClass.DeclareData;
using GeneralClass.Prescription;
using GeneralClass.Prescription.Delcare;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.MedicalBaseClass;
using GeneralClass.Prescription.MedicalOrders;
using GeneralClass.Prescription.Validation;
using GeneralClass.Stock;
using PharmacySystem.ApplicationServiceLayer.UnitOfWork;
using PharmacySystem.Class;
using PharmacySystem.Class.PrescriptionViewModel;
using PharmacySystemInfrastructure.Holiday;
using PharmacySystemInfrastructure.Institution;
using PharmacySystemInfrastructure.Prescription;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class PrescriptionServiceProvider
    {
        public static readonly PrescriptionApplicationService Service =
            PrescriptionApplicationServiceFactory.GetPrescriptionApplicationService();
    }

    public static class PrescriptionApplicationServiceFactory
    {
        public static PrescriptionApplicationService GetPrescriptionApplicationService()
        {
            var prescriptionRepository = new PrescriptionDataBaseRepository();
            var institutionRepository = new InstitutionDatabaseRepository();
            var diseaseCodeRepository = new DiseaseCodeDatabaseRepository();
            var holidayRepository = new HolidayDatabaseRepository();
            var medicalOrderRepository = new MedicalOrderDatabaseRepository();
            var validator = new PrescriptionValidator(institutionRepository, diseaseCodeRepository, holidayRepository);
            return new PrescriptionApplicationService(prescriptionRepository, medicalOrderRepository, validator);
        }
    }

    public class PrescriptionApplicationService
    {
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly EmployeeApplicationService employeeService;
        private readonly CustomerApplicationService customerService;
        private readonly InstitutionApplicationService institutionService;
        private readonly MedicalOrderService medicalOrderService;
        private readonly IPrescriptionValidator prescriptionValidator;
        public PrescriptionApplicationService(IPrescriptionRepository prescriptionRepository, IMedicalOrderRepository medicalOrderRepository, IPrescriptionValidator validator)
        {
            this.prescriptionRepository = prescriptionRepository;
            prescriptionValidator = validator;
            customerService = CustomerApplicationServiceFactory.GetCustomerApplicationService();
            employeeService = EmployeeServiceProvider.Service;
            institutionService = InstitutionServiceProvider.Service;
            medicalOrderService = new MedicalOrderService(medicalOrderRepository);
        }

        public PrescriptionData GetPrescriptionByID(PrescriptionID id)
        {
            var prescription = prescriptionRepository.GetPrescriptionsById(new List<PrescriptionID>{ id }).First();
            prescription.Pharmacist = employeeService.GetAllPharmacist().SingleOrDefault(_ => _.ID.ID.ToString() == prescription.PharmacistID);
            prescription.Patient = customerService.GetCustomerByCusID(prescription.PatientID);
            var ordersID = prescription.MedicalOrders.Select(o => o.ID).ToList();
            var medicalOrders = medicalOrderService.GetMedicalOrdersByID(ordersID);
            foreach (var order in prescription.MedicalOrders)
            {
                var temp = medicalOrders.SingleOrDefault(o => o.ID.Equals(order.ID));
                if (temp is null)
                    continue;
                order.ChineseName = temp.ChineseName;
                order.EnglishName = temp.EnglishName;
                order.Price = temp.Price;
            }
            return prescription;
        }

        /// <summary>
        /// 處方補卡
        /// </summary>
        public bool MakeUpPrescription(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 儲存客戶資料，如果不存在建新的。
        /// </summary>
        public bool SavePatientData(GeneralClass.Customer.Customer customer)
        {
            return customerService.CreateNewCustomer(customer);
        }

        /// <summary>
        /// 算就醫日與調劑日間跨幾個例假
        /// </summary>
        public int CountHoliday(DateTime treatmentDate, DateTime adjustDate)
        {
            return 0;
        }

        public static List<Copayment> GetCopaymentList()
        {
            return CopaymentService.GetCopaymentList();
        }

        public static List<AdjustCase> GetAdjustCaseList()
        {
            return AdjustCaseService.GetAdjustCaseList();
        }

        public List<PrescriptionRecord> SearchPrescription(PrescriptionSearchPattern searchPattern)
        {
            var records = prescriptionRepository.GetPrescriptionRecordsBySearchPattern(searchPattern);
            records.ForEach(r =>
            {
                r.Division = DivisionService.GetDivisionById(r.DivisionID);
                r.AdjustCase = AdjustCaseService.GetAdjustCaseById(r.AdjustCaseID);
            });
            return records;
        }

        public static List<Division> GetDivisionList()
        {
            return DivisionService.GetDivisionList();
        }

        public static List<PaymentCategory> GetPaymentCategoryList()
        {
            return PaymentCategoryService.GetPaymentCategoryList();
        }

        public static List<PrescriptionCase> GetPrescriptionCaseList()
        {
            return PrescriptionCaseService.GetPrescriptionCaseList();
        }

        public static List<SpecialTreat> GetSpecialTreatList()
        {
            return SpecialTreatService.GetSpecialTreatList();
        }

        public PrescriptionErrorCode Save(PrescriptionData data)
        {
            var result = prescriptionValidator.ValidateBeforeRegister(data);
            if (result != PrescriptionErrorCode.Success) return result;
            data.CreateMedicalServiceOrder();
            data.CalculatePoints();
            SavePrescription(data);
            return PrescriptionErrorCode.Success;
        }

        public PrescriptionErrorCode Register(PrescriptionData data)
        {
            var result = prescriptionValidator.ValidateBeforeRegister(data);
            if (result != PrescriptionErrorCode.Success)
            {
                MessageBox.Show(result.GetDescription());
                return result;
            }
            data.CreateMedicalServiceOrder();
            data.CalculatePoints();
            return RegisterPrescription(data) == false ? PrescriptionErrorCode.DBInsertError : result;
        }

        private bool RegisterPrescription(PrescriptionData data)
        {
            try
            {
                using var unitOfWork = new PrescriptionUnitOfWork();
                unitOfWork.PrescriptionRepository.InsertPrescription(data);
                var simpleStocks = new List<InventoryRecord>();
                foreach (var order in data.MedicalOrders)
                {
                    if (order.GetType() != typeof(MedicineOrder) && order.GetType() != typeof(SpecialMaterialOrder))
                        continue;

                    var invID = unitOfWork.StockService.GetInvIDByProIDandWareID(order.ID.ID, data.WareHouseID);
                    var inventoryRecord = InventoryRecordFactory.CreateInventoryRecord(
                        invID,
                        "",
                        (int)(order.TotalAmount * -1),
                        StockType.Prescription,
                        data.ID.ToString(),
                        data.Pharmacist.ID);

                    simpleStocks.Add(inventoryRecord);
                }
                unitOfWork.StockService.SaveRecord(simpleStocks, data.Pharmacist.ID);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }

        private void SavePrescription(PrescriptionData data)
        {
            prescriptionRepository.UpdatePrescription(data);
        }

        public void ClearMedicalServiceOrder(PrescriptionData prescription)
        {
            prescription?.MedicalOrders?.RemoveAll(o => o.Type == MedicalOrderType.MedicalService);
        }

        public void Delete(PrescriptionData prescriptionData)
        {
            prescriptionRepository.DeletePrescription(prescriptionData);
        }

        public Tuple<DeclareDataSummary, List<PrescriptionDeclareDataViewModel>> GetDeclareData(int declareYear, int declareMonth)
        {
            var declarePrescriptions = GetDeclarePrescriptions(declareYear, declareMonth);
            var declareDataViewModelList = new List<PrescriptionDeclareDataViewModel>();
            var institutionsId = declarePrescriptions.Select(p => p.InstitutionID.ID);
            var institutions = institutionService.GetInstitutionsByMultipleId(institutionsId);
            var pharmacists = employeeService.GetAllPharmacist();
            var patients = customerService.GetCustomersByCustomerID(declarePrescriptions.Select(p => p.PatientID).Distinct());
            foreach (var p in declarePrescriptions)
            {
                var vm = new PrescriptionDeclareDataViewModel(p)
                {
                    Institution = institutions.SingleOrDefault(i => i.ID == p.InstitutionID.ID),
                    Pharmacist = pharmacists.FirstOrDefault(_ => _.ID.ID.ToString() == p.PharmacistID),
                    Patient = (PatientDataViewModel)patients.Single(cus => cus.ID.ID == p.PatientID.ID)
                };
                declareDataViewModelList.Add(vm);
            }
            return new Tuple<DeclareDataSummary, List<PrescriptionDeclareDataViewModel>>(new DeclareDataSummary(declarePrescriptions), declareDataViewModelList);
        }

        private List<PrescriptionData> GetDeclarePrescriptions(int declareYear, int declareMonth)
        {
            return prescriptionRepository.GetDeclarePrescriptionsOverview(declareYear, declareMonth);
        }

        public void CreateDeclareFile(IEnumerable<PrescriptionID> declarePrescriptions, string path ,string targetFileName)
        {
            var prescriptions = prescriptionRepository.GetPrescriptionsById(declarePrescriptions);
            var pharmacists = employeeService.GetAllPharmacist();
            var patients = customerService.GetCustomersByCustomerID(prescriptions.Select(p => p.PatientID).Distinct());
            foreach (var prescriptionData in prescriptions)
            {
                prescriptionData.Pharmacist = pharmacists.FirstOrDefault(_ => _.ID.ToString() == prescriptionData.PharmacistID);
                prescriptionData.Patient = patients.Single(p => p.ID.ID == prescriptionData.PatientID.ID);
            }
            var declareFile = new DeclareFile(prescriptions, DeclareMethod.Zip, DeclareCategory.SendCheck);
            prescriptionRepository.SaveDeclareFile(declareFile, path, targetFileName);
        }

        public void ImportDeclareFile(string declareDirectory)
        {
            prescriptionRepository.ImportPrescriptionFromDeclareFiles(declareDirectory);
        }
    }
}
