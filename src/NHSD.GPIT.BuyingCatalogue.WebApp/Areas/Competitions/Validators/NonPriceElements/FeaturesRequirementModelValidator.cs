using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators.NonPriceElements;

public class FeaturesRequirementModelValidator : AbstractValidator<FeaturesRequirementModel>
{
    internal const string MissingRequirementsError = "Enter feature requirements";
    internal const string MissingComplianceError = "Select if this is a must or should requirement";

    public FeaturesRequirementModelValidator()
    {
        RuleFor(x => x.Requirements)
            .NotEmpty()
            .WithMessage(MissingRequirementsError);

        RuleFor(x => x.SelectedCompliance)
            .NotNull()
            .WithMessage(MissingComplianceError);
    }
}
