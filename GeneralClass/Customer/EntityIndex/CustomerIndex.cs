namespace GeneralClass.Customer.EntityIndex
{
    public readonly struct CustomerID
    {
        public static explicit operator CustomerID(int id) => new(id);

        public CustomerID(int id)
        {
            ID = id;
        }

        public int ID { get; }
    }
}
