namespace PharmacySystemInfrastructure.Specification
{
    public readonly struct ValidateRule<T, T1>
    {
        private readonly ISpecification<T1> specification;

        public T ErrorCode { get; }

        public ValidateRule(T errorCode, ISpecification<T1> specification)
        {
            ErrorCode = errorCode;
            this.specification = specification;
        }

        public bool Validate(T1 o)
        {
            return specification.IsSatisfiedBy(o);
        }
    }
}
