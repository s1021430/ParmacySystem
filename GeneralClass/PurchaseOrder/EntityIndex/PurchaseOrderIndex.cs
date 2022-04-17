namespace GeneralClass.PurchaseOrder.EntityIndex
{
    public readonly struct PurchaseOrderID
    {
        public static explicit operator PurchaseOrderID(string id) => new(id);

        public PurchaseOrderID(string id)
        {
            ID = id;
        }

        public string ID { get; }
    }
}
