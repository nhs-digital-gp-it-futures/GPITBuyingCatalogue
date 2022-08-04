using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public sealed class OrderDescriptionModelValidator : AbstractValidator<OrderDescriptionModel>
    {
        public const string OrderDescriptionErrorMessage = "Enter an order description";

        public OrderDescriptionModelValidator()
        {
            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage(OrderDescriptionErrorMessage);
        }
    }
}
