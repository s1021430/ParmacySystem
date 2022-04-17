namespace GeneralClass.Manufactory.EntityIndex
{
    public readonly struct ManufactoryID
    {
        public static explicit operator ManufactoryID(int id) => new(id);
        public ManufactoryID(int id)
        {
            ID = id;
        }

        public int ID { get; }
    }
}
