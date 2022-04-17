using System.Windows;
using GeneralClass.Authority;
using GeneralClass.Customer;
using GeneralClass.Employee;
using GeneralClass.Manufactory;
using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalOrders;
using GeneralClass.Product;
using GeneralClass.PurchaseOrder;
using GeneralClass.PurchaseRequisition;
using GeneralClass.WareHouse;
using PharmacySystemInfrastructure.Customer;
using PharmacySystemInfrastructure.Employee;
using PharmacySystemInfrastructure.Manufactory;
using PharmacySystemInfrastructure.Prescription;
using PharmacySystemInfrastructure.Product;
using PharmacySystemInfrastructure.StoreOrder;
using PharmacySystemInfrastructure.WareHouse;

namespace PharmacySystem
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
    }

    public static class RepositoryProvider
    {
        public static IPurchaseRequisitionRepository PurchaseRequisitionRepository { get; }
        public static IPurchaseOrderRepository PurchaseOrderRepository { get; }
        public static IPrescriptionRepository PrescriptionRepository { get; }
        public static IMedicalOrderRepository MedicalOrderRepository { get; }
        public static IManufactoryRepository ManufactoryRepository { get; }
        public static IEmployeeRepository EmployeeRepository { get; }
        public static ICustomerRepository CustomerRepository { get; }
        public static IProductRepository ProductRepository { get; }
        public static IWareHouseRepository WareHouseRepository { get; }

        static RepositoryProvider()
        {
            PurchaseRequisitionRepository = new PurchaseRequisitionDatabaseRepository();
            PurchaseOrderRepository = new PurchaseOrderDatabaseRepository();
            PrescriptionRepository = new PrescriptionDataBaseRepository();
            MedicalOrderRepository = new MedicalOrderDatabaseRepository();
            ManufactoryRepository = new ManufactoryDatabaseRepository();
            EmployeeRepository = new EmployeeDatabaseRepository();
            CustomerRepository = new CustomerDatabaseRepository();
            ProductRepository = new ProductDatabaseRepository();
            WareHouseRepository = new WareHouseDatabaseRepository();
        }
    }
}
