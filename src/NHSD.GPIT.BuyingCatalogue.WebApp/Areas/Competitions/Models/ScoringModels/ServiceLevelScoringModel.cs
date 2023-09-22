using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class ServiceLevelScoringModel : NavBaseModel
{
    public ServiceLevelScoringModel()
    {
    }

    public ServiceLevelScoringModel(
        Competition competition)
    {
        CompetitionName = competition.Name;

        From = competition.NonPriceElements.ServiceLevel.TimeFrom;
        Until = competition.NonPriceElements.ServiceLevel.TimeUntil;
        ApplicableDays = competition.NonPriceElements.ServiceLevel.ApplicableDays;

        WithSolutions(competition.CompetitionSolutions);
    }

    public string CompetitionName { get; set; }

    public List<SolutionScoreModel> SolutionScores { get; set; }

    public DateTime From { get; set; }

    public DateTime Until { get; set; }

    public string ApplicableDays { get; set; }

    public void WithSolutions(IEnumerable<CompetitionSolution> solutions, bool setScores = true)
    {
        SolutionScores = solutions.OrderBy(x => x.Solution.CatalogueItem.Name)
            .Select(
                x =>
                {
                    var score = x.GetScoreByType(ScoreType.ServiceLevel);

                    return new SolutionScoreModel(
                        x.Solution,
                        setScores ? score?.Score : null,
                        score?.Justification);
                })
            .ToList();
    }
}
