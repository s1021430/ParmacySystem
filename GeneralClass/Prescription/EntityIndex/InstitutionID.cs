namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct InstitutionID
    {
        public static explicit operator InstitutionID(string id) => new(id);

        public InstitutionID(string id)
        {
            ID = id;
        }

        public string ID { get; }
    }
}
