namespace GeneralClass.WareHouse.EntityIndex
{
    public readonly struct WareHouseID
    {
        public static explicit operator WareHouseID(int id) => new(id);
        public WareHouseID(int id)
        {
            ID = id;
        }

        public int ID { get; }

        public bool IsValid()
        {
            return ID >= 0;
        }
    }
}
