using System;
using System.Collections.Generic;
using GeneralClass.Prescription;
using GeneralClass.Prescription.MedicalOrders;

namespace GeneralClass.DomainService.ControlMedicine
{
    public class ControlMedicineDeclareDomainService
    {
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IMedicalOrderRepository medicalOrderRepository;
        public ControlMedicineDeclareDomainService(IPrescriptionRepository _prescriptionRepository, IMedicalOrderRepository _medicalOrderRepository)
        {
            prescriptionRepository = _prescriptionRepository;
            medicalOrderRepository = _medicalOrderRepository;
        }

        public List<PrescriptionData> GetPresriptionUsingControlMedByDate(DateTime startDate,DateTime endDate)
        {
            var contorlMedlist = medicalOrderRepository.GetControlMedicine();

            return prescriptionRepository.GetPrescritionsByMedicalOrderIDAndDate(contorlMedlist, startDate, endDate);
        }

    }
}
