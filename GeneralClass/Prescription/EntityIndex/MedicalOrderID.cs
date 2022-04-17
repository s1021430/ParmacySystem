namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct MedicalOrderID
    {
        public static explicit operator MedicalOrderID(string id) => new(id);

        public MedicalOrderID(string id)
        {
            ID = id;
        }

        public string ID { get; }

        public bool Equals(MedicalOrderID other)
        {
            return ID == other.ID;
        }
    }
}
