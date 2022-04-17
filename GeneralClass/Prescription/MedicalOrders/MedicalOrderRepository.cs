using System.Collections.Generic;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription.MedicalOrders
{
    public interface IMedicalOrderRepository
    {
        List<MedicalOrder> SearchMedicalOrdersByID(string id);
        List<MedicalOrder> GetMedicalOrdersByID(List<MedicalOrderID> medicalOrdersID);
        List<MedicalOrderID> GetControlMedicine();
        MedicalOrder GetVirtualOrder(string id);
    }

}
