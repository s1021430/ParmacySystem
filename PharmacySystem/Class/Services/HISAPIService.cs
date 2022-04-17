using System;
using GeneralClass.Customer;
using GeneralClass.Prescription;
using HisApiBase;
using PharmacySystem.ApplicationServiceLayer;
using PharmacySystem.ViewModel;

namespace PharmacySystem.Class.Services
{
    public static class NHIServiceProvider
    {
        public static NHIService Service = NHIServiceFactory.GetNHIService();
    }
    public static class NHIServiceFactory
    {
        public static NHIService GetNHIService()
        {
            return new NHIService();
        }
    }
    public class NHIService
    {
        private readonly HISAPIService hisAPIService = new HISAPI(ViewModelLocator.IsInDesign);
        private NHICard card;
        public NHICard ReadCard()
        {
            return hisAPIService.GetCardWithBasicData();
        }

        public PatientDataViewModel GetPatientByNHICard()
        {
            card = ReadCard();
            var customerService = CustomerApplicationServiceFactory.GetCustomerApplicationService();
            var newCustomer = new GeneralClass.Customer.Customer(card.BasicData.IDNumber, card.BasicData.Name,
                card.BasicData.Birthday, card.BasicData.Gender);
            return (PatientDataViewModel)customerService.GetCustomersByBasicData(newCustomer);
        }

        public void MakeUpPrescription(PrescriptionData prescription)
        {
            throw new NotImplementedException();
        }
    }
}
