using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class AddImplementationCriteriaModelValidator : AbstractValidator<AddImplementationCriteriaModel>
{
    internal const string EmptyRequirementsError = "Enter implementation requirements";

    public AddImplementationCriteriaModelValidator()
    {
        RuleFor(x => x.Requirements)
            .NotEmpty()
            .WithMessage(EmptyRequirementsError);
    }
}
