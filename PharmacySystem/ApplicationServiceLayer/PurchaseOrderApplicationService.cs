using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using GeneralClass;
using GeneralClass.Customer;
using GeneralClass.Employee.EntityIndex;
using GeneralClass.Manufactory;
using GeneralClass.Manufactory.SearchCondition;
using GeneralClass.PurchaseOrder;
using GeneralClass.PurchaseOrder.EntityIndex;
using GeneralClass.PurchaseOrder.SearchCondition;
using GeneralClass.PurchaseOrder.Validator;
using GeneralClass.PurchaseRequisition;
using GeneralClass.PurchaseRequisition.EntityIndex;
using GeneralClass.Stock;
using GeneralClass.WareHouse;
using PharmacySystem.ApplicationServiceLayer.UnitOfWork;
using PharmacySystem.PurchaseRequisition;
using PharmacySystem.ViewModel;
using PharmacySystemInfrastructure.StoreOrder;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class StoreOrderApplicationServiceFactory
    {
        public static PurchaseOrderApplicationService GetPurchaseOrderApplicationService()
        {
            var purchaseOrderRepository = RepositoryProvider.PurchaseOrderRepository;
            var purchaseRequisitionRepository = RepositoryProvider.PurchaseRequisitionRepository;
            var customerRepository = RepositoryProvider.CustomerRepository;
            var wareHouseRepository = RepositoryProvider.WareHouseRepository;
            var validator = new PurchaseOrderValidator();

            return new PurchaseOrderApplicationService(purchaseOrderRepository, purchaseRequisitionRepository, wareHouseRepository, customerRepository, validator);
        }
    }

    public class PurchaseOrderApplicationService
    {
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IPurchaseRequisitionRepository purchaseRequisitionRepository;
        private readonly IWareHouseRepository wareHouseRepository;
        private readonly IManufactoryRepository manufactoryRepository;
        private readonly ICustomerRepository customerRepository;

        private readonly PurchaseOrderFactory purchaseOrderFactory;
        private readonly IStoreOrderValidator storeOrderValidator;

        public PurchaseOrderApplicationService(IPurchaseOrderRepository purchaseOrderRepository, IPurchaseRequisitionRepository purchaseRequisitionRepository, 
            IWareHouseRepository wareHouseRepository, ICustomerRepository customerRepository, IStoreOrderValidator validator)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.purchaseRequisitionRepository = purchaseRequisitionRepository;
            this.wareHouseRepository = wareHouseRepository;
            this.customerRepository = customerRepository;

            storeOrderValidator = validator;
            
            purchaseOrderFactory = new PurchaseOrderFactory();
        }
        
        public ObservableCollection<PurchaseOrderViewModel> GetDraftPurchaseOrders()
        {
            var purchaseOrders = purchaseOrderRepository.GetDraftPurchaseOrders();
            var wareHouses = wareHouseRepository.GetAllWareHouses();
            var manufactories = manufactoryRepository.GetAllManufactories();
            var customers = customerRepository.GetCustomersByCustomerID(purchaseOrders.Select(_ => _.CustomerID));

            var purchaseOrderViewModels = new ObservableCollection<PurchaseOrderViewModel>();

            foreach (var order in purchaseOrders)
            {
                var manufactory = manufactories.Single(_ => _.ID.Equals(order.ManufactoryID));
                var customer = customers.Single(_ => _.ID.Equals(order.CustomerID));
                var wareHouse = wareHouses.Single(_ => _.ID.Equals(order.WareHouseID));

                var products = new ObservableCollection<OrderProductViewModel>();

                foreach (var product in order.Products)
                    products.Add(new OrderProductViewModel(product, ""));

                purchaseOrderViewModels.Add(new PurchaseOrderViewModel(order, manufactory, customer, wareHouse, products));
            }

            return purchaseOrderViewModels;
        }

        public ObservableCollection<PurchaseOrderViewModel> SearchCompletePurchaseOrders(PurchaseOrderSearchCondition searchCondition)
        {
            if (!searchCondition.HasCondition())
                throw new ArgumentException("Search Condition Can't be empty.");

            try
            {
                var purchaseOrders = purchaseOrderRepository.GetCompletePurchaseOrders(searchCondition);
                var wareHouses = wareHouseRepository.GetAllWareHouses();
                var manufactories = manufactoryRepository.GetAllManufactories();
                var customers = customerRepository.GetCustomersByCustomerID(purchaseOrders.Select(_ => _.CustomerID));

                var purchaseOrderViewModels = new ObservableCollection<PurchaseOrderViewModel>();

                foreach (var order in purchaseOrders)
                {
                    var manufactory = manufactories.Single(_ => _.ID.Equals(order.ManufactoryID));
                    var customer = customers.Single(_ => _.ID.Equals(order.CustomerID));
                    var wareHouse = wareHouses.Single(_ => _.ID.Equals(order.WareHouseID));

                    var products = new ObservableCollection<OrderProductViewModel>();

                    foreach (var product in order.Products)
                        products.Add(new OrderProductViewModel(product, ""));

                    purchaseOrderViewModels.Add(new PurchaseOrderViewModel(order, manufactory, customer, wareHouse, products));
                }

                return purchaseOrderViewModels;
            }
            catch (Exception e)
            {
                //log exception
                return null;
            }
        }

        public PurchaseOrderViewModel CreatePurchaseOrder(EmployeeID creatorID)
        {
            var newPurchaseOrderID = purchaseOrderRepository.GetNewPurchaseOrderID();
            var newOrder = purchaseOrderFactory.CreatePurchaseOrder(newPurchaseOrderID, creatorID);

            var isSuccess = purchaseOrderRepository.Create(newOrder);

            if (!isSuccess) return null;

            var manufactory = manufactoryRepository.GetAllManufactories().Single(_ => _.ID.Equals(newOrder.ManufactoryID));
            var customer = customerRepository.GetCustomerByCusID(newOrder.CustomerID);
            var wareHouse = wareHouseRepository.GetWareHouseByID(newOrder.WareHouseID);

            var products = new ObservableCollection<OrderProductViewModel>();

            foreach (var product in newOrder.Products)
                products.Add(new OrderProductViewModel(product, ""));

            return new PurchaseOrderViewModel(newOrder, manufactory, customer, wareHouse, products);
        }

        public PurchaseOrderErrorCode CompletePurchaseOrder(PurchaseOrderID orderID, EmployeeID completeEmployeeID)
        {
            var order = purchaseOrderRepository.GetPurchaseOrderByID(orderID);

            if (order is null) return PurchaseOrderErrorCode.DontHaveThisOrder;

            var errorCode = storeOrderValidator.ValidateBeforeComplete(order);

            if (errorCode != PurchaseOrderErrorCode.Success)
                return errorCode;

            try
            {
                using var unitOfWork = new PurchaseOrderUnitOfWork();

                order.Complete(completeEmployeeID);
                unitOfWork.PurchaseOrderRepository.Save(order);

                var simpleStocks = new List<InventoryRecord>();

                foreach (var product in order.Products)
                {
                    int invID = unitOfWork.StockService.GetInvIDByProIDandWareID(product.ID.ID,
                        order.WareHouseID.ID);

                    InventoryRecord inventoryRecord = InventoryRecordFactory.CreateInventoryRecord(
                        invID,
                        product.BatchNumber,
                        product.Amount * -1,
                        StockType.Prescription,
                        orderID.ID,
                        completeEmployeeID);

                    simpleStocks.Add(inventoryRecord);
                }

                unitOfWork.StockService.SaveRecord(simpleStocks, completeEmployeeID);

                unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //log exception
                return PurchaseOrderErrorCode.DataNotUpdated;
            }

            return PurchaseOrderErrorCode.Success;
        }

        public bool EditPurchaseOrder(PurchaseOrderViewModel orderViewModel)
        {
            var model = purchaseOrderRepository.GetPurchaseOrderByID(new PurchaseOrderID(orderViewModel.ID));
            
            //Edit Model
            
            return purchaseOrderRepository.Save(model);
        }

        public PurchaseOrderViewModel BringInPurchaseRequisitionData(PurchaseRequisitionID requisitionID, PurchaseOrderID orderID)
        {
            var requisition = purchaseRequisitionRepository.GetPurchaseRequisitionByID(requisitionID);
            var order = purchaseOrderRepository.GetPurchaseOrderByID(orderID);
            
            order.BringIn(requisition);

            var result = purchaseOrderRepository.Save(order);

            if (!result) return null;

            var manufactory = manufactoryRepository.GetAllManufactories().Single(_ => _.ID.Equals(order.ManufactoryID));
            var customer = customerRepository.GetCustomerByCusID(order.CustomerID);
            var wareHouse = wareHouseRepository.GetWareHouseByID(order.WareHouseID);

            var products = new ObservableCollection<OrderProductViewModel>();

            foreach (var product in order.Products)
                products.Add(new OrderProductViewModel(product, ""));

            return new PurchaseOrderViewModel(order, manufactory, customer, wareHouse, products);
        }

        public bool DeletePurchaseOrder(PurchaseOrderID orderID, EmployeeID deleteEmployeeID)
        {


            return purchaseOrderRepository.Delete(orderID);
        }
    }
}
