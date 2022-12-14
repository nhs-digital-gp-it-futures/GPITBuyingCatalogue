using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.AssociatedServicesBilling;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.AssociatedServicesBilling
{
    public sealed class SpecificRequirmentsModelValidator : AbstractValidator<SpecificRequirementsModel>
    {
        public const string NoSelectionErrorMessage = "Select yes if you want to proceed without any specific requirements";

        public SpecificRequirmentsModelValidator()
        {
            RuleFor(x => x.ProceedWithoutSpecificRequirements)
                .NotNull()
                .WithMessage(NoSelectionErrorMessage);
        }
    }
}
