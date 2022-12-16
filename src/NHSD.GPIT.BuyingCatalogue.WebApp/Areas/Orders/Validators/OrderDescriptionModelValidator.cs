using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderDescription;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public sealed class OrderDescriptionModelValidator : AbstractValidator<OrderDescriptionModel>
    {
        public OrderDescriptionModelValidator()
        {
            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter an order description");
        }
    }
}
