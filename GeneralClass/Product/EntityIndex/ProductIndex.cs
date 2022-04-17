namespace GeneralClass.Product.EntityIndex
{
    public readonly struct ProductID
    {
        public static explicit operator ProductID(string id) => new(id);
        public ProductID(string id)
        {
            ID = id;
        }

        public string ID { get; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ID);
        }
    }
}
