namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct PrescriptionCaseID
    {
        public static explicit operator PrescriptionCaseID(string id) => new(id);

        public PrescriptionCaseID(string id)
        {
            ID = id;
        }

        public string ID { get; }
        public bool Equals(PrescriptionCaseID other)
        {
            return ID == other.ID;
        }
    }
}
