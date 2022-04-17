namespace GeneralClass.Prescription.EntityIndex
{
    public readonly struct DiseaseCodeID
    {
        public static explicit operator DiseaseCodeID(string id) => new(id);

        public DiseaseCodeID(string id)
        {
            ID = id;
        }

        public string ID { get; }
    }
}
