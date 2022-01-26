using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices
{
    public sealed class EditAssociatedServiceModelValidator : AbstractValidator<EditAssociatedServiceModel>
    {
        public EditAssociatedServiceModelValidator()
        {
            RuleForEach(m => m.OrderItem.ServiceRecipients)
                .ChildRules(v => v.RuleFor(m => m.Quantity)
                    .IsValidQuantity());

            RuleFor(m => m.OrderItem.AgreedPrice)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Enter an agreed price")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price cannot be negative")
                .Must(NotExceedTheListPrice)
                .WithMessage("Price cannot be greater than list price");

            RuleFor(m => m.EstimationPeriod)
                .NotNull()
                .WithMessage("Time unit is required")
                .When(m => m.OrderItem.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand);
        }

        private static bool NotExceedTheListPrice(EditAssociatedServiceModel model, decimal? price)
            => price <= model.OrderItem.CataloguePrice.Price;
    }
}
