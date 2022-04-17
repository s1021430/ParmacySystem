using System;
using GeneralClass.Customer;
using GeneralClass.Customer.Validator.Specification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GeneralClassTest.Person
{
    [TestClass]
    public class CustomerValidationUnitTests
    {
        [TestMethod]
        public void Test_驗證_身分證空值()
        {
            var specification = new IdNumberSpecification();
            var stub = Substitute.For<Customer>();
            stub.IDNumber = string.Empty;
            var result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
            stub.IDNumber = null;
            result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_驗證_身分證格式錯誤()
        {
            var specification = new IdNumberSpecification();
            var stub = Substitute.For<Customer>();
            stub.IDNumber = "A111111111";
            var result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_驗證_生日未填寫()
        {
            var specification = new BirthdaySpecification();
            var stub = Substitute.For<Customer>();
            stub.Birthday = null;
            var result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_驗證_生日超出範圍()
        {
            var specification = new BirthdaySpecification();
            var stub = Substitute.For<Customer>();
            stub.Birthday = DateTime.Today.AddDays(1);
            var futureResult = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, futureResult); 
            stub.Birthday = DateTime.Today.AddYears(-120);
            var pastResult = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, pastResult);
        }

        [TestMethod]
        public void Test_驗證_姓名未填寫()
        {
            var specification = new NameSpecification();
            var stub = Substitute.For<Customer>();
            stub.Name = string.Empty;
            var result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
            stub.Name = null;
            result = specification.IsSatisfiedBy(stub);
            Assert.AreEqual(false, result);
        }
    }
}
