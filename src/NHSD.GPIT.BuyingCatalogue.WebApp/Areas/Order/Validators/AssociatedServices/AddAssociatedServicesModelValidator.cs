using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices
{
    public class AddAssociatedServicesModelValidator : AbstractValidator<AddAssociatedServicesModel>
    {
        public const string AdditionalServicesRequiredMissingErrorMessage = "Select yes if you want to add any Associated Services";

        public AddAssociatedServicesModelValidator()
        {
            RuleFor(x => x.AdditionalServicesRequired)
                .NotEmpty()
                .WithMessage(AdditionalServicesRequiredMissingErrorMessage);
        }
    }
}
