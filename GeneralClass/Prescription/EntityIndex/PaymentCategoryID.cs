namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct PaymentCategoryID
    {
        public static explicit operator PaymentCategoryID(string id) => new(id);

        public PaymentCategoryID(string id)
        {
            ID = id;
        }

        public string ID { get; }
        public bool Equals(PaymentCategoryID other)
        {
            return ID == other.ID;
        }
    }
}
