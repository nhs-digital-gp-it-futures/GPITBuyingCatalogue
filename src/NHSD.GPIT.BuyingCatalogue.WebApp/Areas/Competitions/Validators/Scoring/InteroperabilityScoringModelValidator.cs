﻿using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators.Scoring;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

[ExcludeFromCodeCoverage(Justification = "Configures a sub-validator to use on a property and doesn't have validation logic")]
public class InteroperabilityScoringModelValidator : AbstractValidator<InteroperabilityScoringModel>
{
    public InteroperabilityScoringModelValidator()
    {
        RuleForEach(x => x.SolutionScores)
            .Cascade(CascadeMode.Continue)
            .SetValidator(new SolutionScoreModelValidator());
    }
}
