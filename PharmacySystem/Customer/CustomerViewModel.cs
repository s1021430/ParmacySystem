using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PharmacySystem.Customer
{
    public class CustomerViewModel : ObservableObject
    {
        public static implicit operator CustomerViewModel(GeneralClass.Customer.Customer customer) => new CustomerViewModel(customer);

        private int id;
        public int ID
        {
            get => id;
            set => Set(ref id, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        private CustomerViewModel(GeneralClass.Customer.Customer customer)
        {
            id = customer.ID.ID;
            name = customer.Name;
        }
    }
}
