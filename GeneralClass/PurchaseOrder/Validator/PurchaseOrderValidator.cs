using System.Collections.Generic;
using GeneralClass.PurchaseOrder.Validator.Specification;
using GeneralClass.Specification;

namespace GeneralClass.PurchaseOrder.Validator
{
    public enum PurchaseOrderErrorCode
    {
        UnValidate,
        Success,
        WareHouseIsInvalid,
        OrderAlreadyComplete,
        ProductOrderDontHaveAmount,
        ProductPriceIsNegative,
        NoProduct,
        DataNotUpdated,
        DontHaveThisOrder
    }

    public interface IStoreOrderValidator
    {
        PurchaseOrderErrorCode ValidateBeforeComplete(PurchaseOrder order);
    }

    public class PurchaseOrderValidator : IStoreOrderValidator
    {
        private readonly List<ValidateRule<PurchaseOrderErrorCode, PurchaseOrder>> orderSpecificationsBeforeComplete;
        private readonly List<ValidateRule<PurchaseOrderErrorCode, OrderProduct>> productSpecificationsBeforeComplete;

        public PurchaseOrderValidator()
        {
            orderSpecificationsBeforeComplete = new List<ValidateRule<PurchaseOrderErrorCode, PurchaseOrder>>()
            {
                new(PurchaseOrderErrorCode.WareHouseIsInvalid, new WareHouseValidSpecification()),
                new(PurchaseOrderErrorCode.OrderAlreadyComplete, new IsOrderCompleteSpecification().Not()),
                new(PurchaseOrderErrorCode.NoProduct, new IsOrderProductEmptySpecification().Not())
            };

            productSpecificationsBeforeComplete = new List<ValidateRule<PurchaseOrderErrorCode, OrderProduct>>()
            {
                new(PurchaseOrderErrorCode.ProductOrderDontHaveAmount, new IsProductHasAmountSpecification()),
                new(PurchaseOrderErrorCode.ProductPriceIsNegative, new IsProductPriceIsNegativeSpecification().Not()),
            };
        }
        
        public PurchaseOrderErrorCode ValidateBeforeComplete(PurchaseOrder order)
        {
            foreach (var validateRule in orderSpecificationsBeforeComplete)
            {
                if (!validateRule.Validate(order))
                    return validateRule.ErrorCode;
            }

            foreach (var product in order.Products)
            {
                foreach (var validateRule in productSpecificationsBeforeComplete)
                {
                    if (!validateRule.Validate(product))
                        return validateRule.ErrorCode;
                }
            }

            return PurchaseOrderErrorCode.Success;
        }
    }
}
