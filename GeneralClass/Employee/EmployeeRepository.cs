using System.Collections.Generic;
using GeneralClass.Employee.EntityIndex;

namespace GeneralClass.Employee
{
    public interface IEmployeeRepository
    {
        bool Save(Employee employee);
        bool Create(Employee employee);
        bool Delete(EmployeeID id);
        List<Employee> GetAllEmployee();

        Employee Login(string account, string password);
    }
}
