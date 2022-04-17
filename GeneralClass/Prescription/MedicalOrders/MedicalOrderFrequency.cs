namespace GeneralClass.Prescription.MedicalOrders
{
    public struct MedicalOrderFrequency
    {
        public MedicalOrderFrequency(string id, string description, int timesPerDay)
        {
            Id = id;
            Description = description;
            TimesPerDay = timesPerDay;
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public int TimesPerDay { get; set; }
    }
}
