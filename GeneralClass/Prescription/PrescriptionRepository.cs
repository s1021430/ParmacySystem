using System;
using System.Collections.Generic;
using GeneralClass.DeclareData;
using GeneralClass.Prescription.EntityIndex;

namespace GeneralClass.Prescription
{
    public interface IPrescriptionRepository
    {
        void InsertPrescription(PrescriptionData pres);

        void DeletePrescription(PrescriptionData pres);

        bool UpdatePrescription(PrescriptionData pres);

        List<PrescriptionRecord> GetPrescriptionRecordsBySearchPattern(PrescriptionSearchPattern searchPattern);

        List<PrescriptionData> GetDeclarePrescriptionsOverview(int year, int month);

        void SaveDeclareFile(DeclareFile declareFile, string path, string declareFileName);

        void ImportPrescriptionFromDeclareFiles(string declareDirectory);

        List<PrescriptionData> GetPrescriptionsById(IEnumerable<PrescriptionID> prescriptionsId);

        List<PrescriptionData> GetPrescritionsByMedicalOrderIDAndDate(IEnumerable<MedicalOrderID> orderID,DateTime startDate,DateTime endDate);
    }
}
