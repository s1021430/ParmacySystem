using System.Collections.Generic;
using GeneralClass.Prescription.MedicalOrders;
using PharmacySystemInfrastructure.Prescription;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class MedicalOrderApplicationServiceProvider
    {
        public static readonly MedicalOrderApplicationService Service =
            MedicalOrderApplicationServiceFactory.GetMedicalOrderApplicationService();
    }

    public static class MedicalOrderApplicationServiceFactory
    {
        public static MedicalOrderApplicationService GetMedicalOrderApplicationService()
        {
            var medicalOrderRepository = new MedicalOrderDatabaseRepository();
            return new MedicalOrderApplicationService(medicalOrderRepository);
        }
    }

    public class MedicalOrderApplicationService
    {
        private readonly IMedicalOrderRepository medicalOrderRepository;

        public MedicalOrderApplicationService(IMedicalOrderRepository repository)
        {
            medicalOrderRepository = repository;
        }

        public List<MedicalOrder> SearchMedicalOrdersByID(string id)
        {
            var searchString = id.ToUpper();
            var virtualOrder = medicalOrderRepository.GetVirtualOrder(id);
            return virtualOrder != null ? new List<MedicalOrder> { virtualOrder } :
                medicalOrderRepository.SearchMedicalOrdersByID(searchString);
        }
    }
}
