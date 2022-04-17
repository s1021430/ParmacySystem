using GeneralClass.Customer;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using GeneralClass.Prescription.Validation.Specification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PharmacySystemInfrastructure.Institution;

namespace GeneralClassTest.Prescription
{
    [TestClass]
    public class PrescriptionValidationUnitTests
    {
        [TestMethod]
        public void Test_處方登錄_健保法規_醫療院所未填寫()
        {
            var specification = new InstitutionSpecification(new InstitutionDatabaseRepository());
            var stub = Substitute.For<PrescriptionData>();
            stub.InstitutionID = new InstitutionID(string.Empty);
            stub.Patient = new Customer();
            var result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
        }
    }
}
