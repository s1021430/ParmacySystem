using System;
using System.Collections.Generic;
using System.Linq;
using GeneralClass.Customer;
using GeneralClass.Customer.EntityIndex;
using PharmacySystemInfrastructure.Customer;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class CustomerApplicationServiceFactory
    {
        public static CustomerApplicationService GetCustomerApplicationService()
        {
            var repository = new CustomerDatabaseRepository();
            return new CustomerApplicationService(repository);
        }
    }

    public class CustomerApplicationService
    {
        private readonly ICustomerRepository repository;

        public CustomerApplicationService(ICustomerRepository customerRepository)
        {
            repository = customerRepository;
        }

        public GeneralClass.Customer.Customer GetCustomerByCusID(CustomerID cusID)
        {
            return repository.GetCustomerByCusID(cusID);
        }

        public List<GeneralClass.Customer.Customer> GetCustomersByIDNumber(string idNumber)
        {
            return repository.GetCustomerByIDNumber(idNumber);
        }

        public List<GeneralClass.Customer.Customer> GetCustomersByBirthday(DateTime birthday)
        {
            return repository.GetCustomerByBirthday(birthday);
        }

        public List<GeneralClass.Customer.Customer> GetCustomersByName(string name)
        {
            return repository.GetCustomerByName(name);
        }

        public List<GeneralClass.Customer.Customer> GetCustomersByPhoneNumber(string phoneNumber)
        {
            return repository.GetCustomerByPhoneNumber(phoneNumber);
        }

        public GeneralClass.Customer.Customer GetCustomersByBasicData(GeneralClass.Customer.Customer customer)
        {
            var customers = repository.GetCustomerByIDNumber(customer.IDNumber);
            if (customers.Any())
                return customers.First();
            CreateNewCustomer(customer);
            return customer;
        }

        public bool CreateNewCustomer(GeneralClass.Customer.Customer customer)
        {
            return repository.CreateOrUpdateCustomer(customer);
        }

        public List<GeneralClass.Customer.Customer> GetCustomersByCustomerID(IEnumerable<CustomerID> customersID)
        {
            return repository.GetCustomersByCustomerID(customersID);
        }
    }
}
