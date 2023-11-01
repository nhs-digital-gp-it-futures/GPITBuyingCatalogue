using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class FeaturesScoringModel : NavBaseModel
{
    public FeaturesScoringModel()
    {
    }

    public FeaturesScoringModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        Features = competition.NonPriceElements?.Features?.ToList();

        WithSolutions(competition.CompetitionSolutions);
    }

    public string CompetitionName { get; set; }

    public List<FeaturesCriteria> Features { get; set; }

    public List<FeaturesCriteria> MustFeatures => Features.Where(x => x.Compliance == CompliancyLevel.Must).ToList();

    public List<FeaturesCriteria> ShouldFeatures => Features.Where(x => x.Compliance == CompliancyLevel.Should).ToList();

    public List<SolutionScoreModel> SolutionScores { get; set; }

    public string PdfUrl { get; set; }

    public void WithSolutions(IEnumerable<CompetitionSolution> solutions, bool setScores = true)
    {
        SolutionScores = solutions.OrderBy(x => x.Solution.CatalogueItem.Name)
            .Select(
                x =>
                {
                    var score = x.GetScoreByType(ScoreType.Features);

                    return new SolutionScoreModel(
                        x.Solution,
                        setScores ? score?.Score : null,
                        score?.Justification);
                })
            .ToList();
    }
}
