using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class SolutionJustificationModelValidator : AbstractValidator<SolutionJustificationModel>
{
    public SolutionJustificationModelValidator()
    {
        RuleFor(x => x.Justification)
            .NotEmpty()
            .WithMessage("Explain why the solution was not included in your shortlist");
    }
}
