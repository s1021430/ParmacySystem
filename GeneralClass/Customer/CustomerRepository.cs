using System;
using System.Collections.Generic;
using GeneralClass.Customer.EntityIndex;

namespace GeneralClass.Customer
{
    public interface ICustomerRepository
    {
        bool Save(Customer customer);
        bool Create(Customer customer);
        bool Delete(CustomerID id);
        Customer GetCustomerByCusID(CustomerID cusID);
        List<Customer> GetCustomerByIDNumber(string idNumber);
        List<Customer> GetCustomerByBirthday(DateTime birthday);
        List<Customer> GetCustomerByName(string name);
        List<Customer> GetCustomerByPhoneNumber(string phoneNumber);
        bool CreateOrUpdateCustomer(Customer customer);
        List<Customer> GetCustomersByCustomerID(IEnumerable<CustomerID> customersID);
    }
}