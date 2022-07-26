using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.AssociatedServicesBilling;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServicesBilling
{
    public class ReviewBillingModelValidator : AbstractValidator<ReviewBillingModel>
    {
        public const string NoSelectionErrorMessage = "Select yes if you want to proceed with this payment schedule";

        public ReviewBillingModelValidator()
        {
            RuleFor(x => x.UseDefaultBilling)
                .NotNull()
                .WithMessage(NoSelectionErrorMessage);
        }
    }
}
