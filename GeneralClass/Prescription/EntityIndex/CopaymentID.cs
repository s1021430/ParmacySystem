namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct CopaymentID
    {
        public static explicit operator CopaymentID(string id) => new(id);

        public CopaymentID(string id)
        {
            ID = id;
        }

        public string ID { get; }

        public bool Equals(CopaymentID other)
        {
            return ID == other.ID;
        }
    }
}
