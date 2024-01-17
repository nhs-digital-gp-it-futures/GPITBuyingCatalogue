using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public class DetermineAssociatedServiceTypeModelValidator : AbstractValidator<DetermineAssociatedServiceTypeModel>
    {
        internal const string OrderTypeRequiredErrorMessage = "Select the type of Associated Service you want to order";

        public DetermineAssociatedServiceTypeModelValidator()
        {
            RuleFor(m => m.OrderType)
                .NotNull()
                .WithMessage(OrderTypeRequiredErrorMessage);
        }
    }
}
