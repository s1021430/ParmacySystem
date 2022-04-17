namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct SpecialTreatID
    {
        public static explicit operator SpecialTreatID(string id) => new(id);

        public SpecialTreatID(string id)
        {
            ID = id;
        }

        public string ID { get; }
        public bool Equals(SpecialTreatID other)
        {
            return ID == other.ID;
        }
    }
}
