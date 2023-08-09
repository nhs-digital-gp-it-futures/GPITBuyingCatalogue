using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class SolutionScoreModel
{
    public SolutionScoreModel()
    {
    }

    public SolutionScoreModel(
        Solution solution,
        int? score)
    {
        SolutionId = solution.CatalogueItemId;
        Solution = solution;
        Score = score;
    }

    public CatalogueItemId SolutionId { get; set; }

    public Solution Solution { get; set; }

    [ModelBinder(typeof(NumberModelBinder))]
    public int? Score { get; set; }
}
