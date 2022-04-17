using System;
using GeneralClass.Person;
using GeneralClass.Specification;

namespace GeneralClass.Customer.Validator.Specification
{
    public class BirthdaySpecification : CompositeSpecification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer?.Birthday != null && customer.Birthday < DateTime.Now 
                && ((DateTime)customer.Birthday).AddYears(120) > DateTime.Now;
        }
    }

    public class IdNumberSpecification : CompositeSpecification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer is not null && PersonService.IdNumberValidation(customer.IDNumber) 
                   || PersonService.ResidentIDValidation(customer.IDNumber);
        }
    }

    public class NameSpecification : CompositeSpecification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer is not null && PersonService.NameValidation(customer.Name);
        }
    }
}
