using GeneralClass.Employee.EntityIndex;

namespace GeneralClass.Employee
{
    public class Employee
    {
        public Employee() { }
        
        public EmployeeID ID { get; set; }
        public string Name { get; set; } 
        public string IDNumber { get; set; }
        public string Note { get; set; }
        public string Account { get; set; }
        public string Password { get; set; } 
        public int AUTH_ID { get; set; }
        public bool IsEnable { get; set; }
    }
}
