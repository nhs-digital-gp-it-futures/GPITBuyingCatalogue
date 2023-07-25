﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class ImplementationScoringModel : NavBaseModel
{
    public ImplementationScoringModel()
    {
    }

    public ImplementationScoringModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        Implementation = competition.NonPriceElements.Implementation.Requirements;

        WithSolutions(competition.CompetitionSolutions);
    }

    public string CompetitionName { get; set; }

    public string Implementation { get; set; }

    public List<SolutionScoreModel> SolutionScores { get; set; }

    public ImplementationScoringModel WithSolutions(IEnumerable<CompetitionSolution> solutions)
    {
        SolutionScores = solutions.OrderBy(x => x.Solution.CatalogueItem.Name)
            .Select(
                x => new SolutionScoreModel(
                    x.Solution,
                    x.GetScoreByType(ScoreType.Implementation)?.Score))
            .ToList();

        return this;
    }
}
