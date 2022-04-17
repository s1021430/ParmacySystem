namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct PrescriptionID
    {
        public static explicit operator PrescriptionID(int id) => new(id);

        public PrescriptionID(int id)
        {
            ID = id;
        }

        public int ID { get; }

        public bool Equals(PrescriptionID prescriptionID)
        {
            return ID == prescriptionID.ID;
        }
    }
}
