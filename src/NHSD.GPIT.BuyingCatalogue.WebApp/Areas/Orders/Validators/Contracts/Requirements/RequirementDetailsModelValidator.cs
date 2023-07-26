using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.Requirements
{
    public class RequirementDetailsModelValidator : AbstractValidator<RequirementDetailsModel>
    {
        public const string DetailsRequiredErrorMessage = "Enter the requirement";
        public const string AssociatedServiceRequiredErrorMessage = "Enter an Associated Service name";

        public RequirementDetailsModelValidator()
        {
            RuleFor(x => x.SelectedOrderItemId)
                .NotEmpty()
                .WithMessage(AssociatedServiceRequiredErrorMessage);

            RuleFor(x => x.Details)
                .NotEmpty()
                .WithMessage(DetailsRequiredErrorMessage);
        }
    }
}
