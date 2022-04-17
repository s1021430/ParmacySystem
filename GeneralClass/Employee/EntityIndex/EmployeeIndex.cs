namespace GeneralClass.Employee.EntityIndex
{
    public readonly struct EmployeeID
    {
        public static explicit operator EmployeeID(int id) => new(id);

        public EmployeeID(int id)
        {
            ID = id;
        }

        public int ID { get; }
    }
}
