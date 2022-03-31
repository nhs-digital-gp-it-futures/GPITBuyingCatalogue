using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public class OrderItemTypeModelValidator : AbstractValidator<OrderItemTypeModel>
    {
        internal const string SelectedOrderItemTypeError = "Select Catalogue Solution or Associated Service";

        public OrderItemTypeModelValidator()
        {
            RuleFor(m => m.SelectedOrderItemType)
                .NotNull()
                .WithMessage(SelectedOrderItemTypeError);
        }
    }
}
