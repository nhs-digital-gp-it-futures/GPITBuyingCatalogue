using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Shared
{
    public sealed class CreateOrderItemModelValidator : AbstractValidator<CreateOrderItemModel>
    {
        public CreateOrderItemModelValidator()
        {
            RuleFor(m => m.AgreedPrice)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Enter an agreed price")
                .GreaterThan(0)
                .WithMessage("Price cannot be negative")
                .Must(NotExceedTheListPrice)
                .WithMessage("Price cannot be greater than list price");

            RuleForEach(m => m.ServiceRecipients)
                .SetValidator(m => new OrderItemRecipientModelValidator(m.CommencementDate));
        }

        private static bool NotExceedTheListPrice(CreateOrderItemModel model, decimal? price)
            => price <= model.CataloguePrice.Price;
    }
}
