using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

[ExcludeFromCodeCoverage(Justification = "Configures a sub-validator to use on a property and doesn't have validation logic")]
public class ServiceLevelScoringModelValidator : AbstractValidator<ServiceLevelScoringModel>
{
    public ServiceLevelScoringModelValidator()
    {
        RuleForEach(x => x.SolutionScores)
            .Cascade(CascadeMode.Continue)
            .SetValidator(new SolutionScoreModelValidator());
    }
}
