using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GeneralClass.Customer;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem.SearchDialogs
{
    public class CustomerSearchDialogViewModel : ViewModelBase
    {
        private readonly CustomerApplicationService customerService;
        private bool customerSelected;
        public bool CustomerSelected
        {
            get => customerSelected;
            set
            {
                Set(() => CustomerSelected, ref customerSelected, value);
            }
        }

        private ObservableCollection<GeneralClass.Customer.Customer> result = new ObservableCollection<GeneralClass.Customer.Customer>();
        public ObservableCollection<GeneralClass.Customer.Customer> Result
        {
            get => result;
            private set
            {
                Set(() => Result, ref result, value);
            }
        }

        private GeneralClass.Customer.Customer selectedCustomer;
        public GeneralClass.Customer.Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                Set(() => SelectedCustomer, ref selectedCustomer, value);
                if (value != null)
                    CustomerSelected = true;
            }
        }

        public CustomerSearchDialogViewModel(CustomerSearchPattern searchPattern)
        {
            customerService = CustomerApplicationServiceFactory.GetCustomerApplicationService();
            GetCustomers(searchPattern);
        }

        private void GetCustomers(CustomerSearchPattern searchPattern)
        {
            switch (searchPattern.Condition)
            {
                case CustomerSearchCondition.IDNumber:
                    if (searchPattern.Content is string idNumber)
                        Result = new ObservableCollection<GeneralClass.Customer.Customer>(customerService.GetCustomersByIDNumber(idNumber));
                    break;
                case CustomerSearchCondition.Name:
                    if (searchPattern.Content is string name)
                        Result = new ObservableCollection<GeneralClass.Customer.Customer>(customerService.GetCustomersByName(name));
                    break;
                case CustomerSearchCondition.Birthday:
                    if (searchPattern.Content is DateTime birthday)
                        Result = new ObservableCollection<GeneralClass.Customer.Customer>(customerService.GetCustomersByBirthday(birthday));
                    break;
                case CustomerSearchCondition.FirstPhoneNumber:
                case CustomerSearchCondition.SecondPhoneNumber:
                    if (searchPattern.Content is string phoneNumber)
                        Result = new ObservableCollection<GeneralClass.Customer.Customer>(customerService.GetCustomersByPhoneNumber(phoneNumber));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
