using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class ServiceLevelScoringModelValidator : AbstractValidator<ServiceLevelScoringModel>
{
    public ServiceLevelScoringModelValidator()
    {
        RuleForEach(x => x.SolutionScores)
            .Cascade(CascadeMode.Continue)
            .SetValidator(new SolutionScoreModelValidator());
    }
}
