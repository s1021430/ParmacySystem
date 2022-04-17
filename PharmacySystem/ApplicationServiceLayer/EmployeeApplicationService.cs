using System.Collections.Generic;
using System.Linq;
using GeneralClass.Employee;
using PharmacySystemInfrastructure.Employee;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class EmployeeServiceProvider
    {
        public static readonly EmployeeApplicationService Service = EmployeeApplicationServiceFactory.GetEmployeeApplicationService();
    }

    public static class EmployeeApplicationServiceFactory
    {
        public static EmployeeApplicationService GetEmployeeApplicationService()
        {
            var repository = new EmployeeDatabaseRepository();

            return new EmployeeApplicationService(repository);
        }
    }

    public class EmployeeApplicationService
    {
        private readonly IEmployeeRepository repository;

        public EmployeeApplicationService(IEmployeeRepository employeeRepository)
        {
            repository = employeeRepository;
        }

        public List<Employee> GetAllPharmacist()
        {
            return repository.GetAllEmployee().Where(_ => _.AUTH_ID == 1).ToList();
        }
    }
}
