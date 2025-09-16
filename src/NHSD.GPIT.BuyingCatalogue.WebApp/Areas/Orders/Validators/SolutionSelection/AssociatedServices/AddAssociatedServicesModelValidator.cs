using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.AssociatedServices
{
    public class AddAssociatedServicesModelValidator : AbstractValidator<AddAssociatedServicesModel>
    {
        public const string AdditionalServicesRequiredMissingErrorMessage = "Select yes if you want to add any associated services";

        public AddAssociatedServicesModelValidator()
        {
            RuleFor(x => x.AdditionalServicesRequired)
                .NotEmpty()
                .WithMessage(AdditionalServicesRequiredMissingErrorMessage);
        }
    }
}
