using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class SolutionPriceModel
{
    public SolutionPriceModel(
        CompetitionSolution solution)
    {
        CatalogueItemId = solution.SolutionId;
        Name = solution.Solution.CatalogueItem.Name;
    }

    public CatalogueItemId CatalogueItemId { get; set; }

    public string Name { get; set; }

    public double? Price { get; set; }

    public TaskProgress Progress { get; set; } = TaskProgress.NotStarted;
}
