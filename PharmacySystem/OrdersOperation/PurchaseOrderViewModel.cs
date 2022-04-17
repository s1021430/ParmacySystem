using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GeneralClass.PurchaseOrder;
using PharmacySystem.Customer;
using PharmacySystem.Manufactory;
using PharmacySystem.Product;
using PharmacySystem.WareHouse;

namespace PharmacySystem.PurchaseRequisition
{
    public class PurchaseOrderViewModel : ObservableObject
    {
        private string id;
        public string ID
        {
            get => id;
            set => Set(ref id, value);
        }

        private string requisitionID;
        public string RequisitionID
        {
            get => requisitionID;
            set => Set(ref requisitionID, value);
        }

        private ManufactoryViewModel manufactory;
        public ManufactoryViewModel Manufactory
        {
            get => manufactory;
            set => Set(ref manufactory, value);
        }

        private CustomerViewModel customer;
        public CustomerViewModel Customer
        {
            get => customer;
            set => Set(ref customer, value);
        }

        private WareHouseViewModel wareHouse;
        public WareHouseViewModel WareHouse
        {
            get => wareHouse;
            set => Set(ref wareHouse, value);
        }

        private string note;
        public string Note
        {
            get => note;
            set => Set(ref note, value);
        }

        private ObservableCollection<OrderProductViewModel> products;
        public ObservableCollection<OrderProductViewModel> Products
        {
            get => products;
            set => Set(ref products, value);
        }

        public PurchaseOrderViewModel(PurchaseOrder order, ManufactoryViewModel manufactory, CustomerViewModel customer, WareHouseViewModel wareHouse, ObservableCollection<OrderProductViewModel> products)
        {
            id = order.ID.ID;
            requisitionID = order.RequisitionID.ID;
            this.products = products;
            this.manufactory = manufactory;
            this.customer = customer;
            this.wareHouse = wareHouse;
            note = order.Note;
        }
    }

    public class OrderProductViewModel : ProductViewModel
    {
        private int amount;
        public int Amount
        {
            get => amount;
            set => Set(ref amount, value);
        }

        private int freeAmount;
        public int FreeAmount
        {
            get => freeAmount;
            set => Set(ref freeAmount, value);
        }

        private decimal price;
        public decimal Price
        {
            get => price;
            set => Set(ref price, value);
        }

        private decimal subtotal;
        public decimal Subtotal
        {
            get => subtotal;
            set => Set(ref subtotal, value);
        }

        private DateTime validDate;
        public DateTime ValidDate
        {
            get => validDate;
            set => Set(ref validDate, value);
        }

        private string batchNumber;
        public string BatchNumber
        {
            get => batchNumber;
            set => Set(ref batchNumber, value);
        }

        private string note;
        public string Note
        {
            get => note;
            set => Set(ref note, value);
        }
        
        public OrderProductViewModel(OrderProduct product, string productName)
        {
            id = product.ID.ID;
            name = productName;
            amount = product.Amount;
            freeAmount = product.FreeAmount;
            price = product.Price;
            subtotal = product.SubTotal;
            validDate = product.ValidDate;
            batchNumber = product.BatchNumber;
            note = product.Note;
        }
    }
}
