using System.Linq;
using GeneralClass.Specification;

namespace GeneralClass.PurchaseOrder.Validator.Specification
{
    public class WareHouseValidSpecification : CompositeSpecification<PurchaseOrder>
    {
        public override bool IsSatisfiedBy(PurchaseOrder order)
        {
            return order.WareHouseID.IsValid();
        }
    }

    public class IsOrderCompleteSpecification : CompositeSpecification<PurchaseOrder>
    {
        public override bool IsSatisfiedBy(PurchaseOrder order)
        {
            return order.Status == DataStatus.Complete;
        }
    }

    public class IsOrderProductEmptySpecification : CompositeSpecification<PurchaseOrder>
    {
        public override bool IsSatisfiedBy(PurchaseOrder order)
        {
            return !order.Products.Any();
        }
    }

    public class IsProductHasAmountSpecification : CompositeSpecification<OrderProduct>
    {
        public override bool IsSatisfiedBy(OrderProduct product)
        {
            return product.Amount > 0 || product.FreeAmount > 0;
        }
    }

    public class IsProductAmountIsZeroSpecification : CompositeSpecification<OrderProduct>
    {
        public override bool IsSatisfiedBy(OrderProduct product)
        {
            return product.Amount == 0 || product.FreeAmount == 0;
        }
    }

    public class IsProductPriceIsNegativeSpecification : CompositeSpecification<OrderProduct>
    {
        public override bool IsSatisfiedBy(OrderProduct product)
        {
            return product.Price < 0;
        }
    }
}
