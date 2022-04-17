namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct DivisionID
    {
        public static explicit operator DivisionID(string id) => new(id);

        public DivisionID(string id)
        {
            ID = id;
        }

        public string ID { get; }

        public bool Equals(DivisionID other)
        {
            return ID == other.ID;
        }
    }
}
