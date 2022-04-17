namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct AdjustCaseID
    {
        public static explicit operator AdjustCaseID(string id) => new(id);

        public AdjustCaseID(string id)
        {
            ID = id;
        }

        public string ID { get; }

        public bool Equals(AdjustCaseID other)
        {
            return ID == other.ID;
        }
    }
}
