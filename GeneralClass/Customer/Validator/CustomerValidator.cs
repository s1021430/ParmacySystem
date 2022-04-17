using System.Collections.Generic;
using GeneralClass.Customer.Validator.Specification;
using GeneralClass.Specification;

namespace GeneralClass.Customer.Validator
{
    public enum CustomerErrorCode
    {
        BirthdayInvalid,
        IdNumberInvalid,
        Success,
    }

    public interface ICustomerValidator
    {
        CustomerErrorCode ValidateWhenPrescriptionSave(Customer customer);
    }

    public class CustomerValidator : ICustomerValidator
    {
        private readonly List<ValidateRule<CustomerErrorCode, Customer>> specificationsBeforePrescriptionSave;

        public CustomerValidator()
        {
            specificationsBeforePrescriptionSave = new List<ValidateRule<CustomerErrorCode, Customer>>
            {
                new(CustomerErrorCode.BirthdayInvalid, new BirthdaySpecification()),
                new(CustomerErrorCode.IdNumberInvalid, new IdNumberSpecification()),
            };
        }

        public CustomerErrorCode ValidateWhenPrescriptionSave(Customer customer)
        {
            foreach (var validateRule in specificationsBeforePrescriptionSave)
            {
                if (!validateRule.Validate(customer))
                    return validateRule.ErrorCode;
            }
            return CustomerErrorCode.Success;
        }
    }
}
