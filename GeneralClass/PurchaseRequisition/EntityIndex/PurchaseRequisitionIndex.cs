namespace GeneralClass.PurchaseRequisition.EntityIndex
{
    public readonly struct PurchaseRequisitionID
    {
        public static explicit operator PurchaseRequisitionID(string id) => new(id);
        public PurchaseRequisitionID(string id)
        {
            ID = id;
        }

        public string ID { get; }
    }
}
