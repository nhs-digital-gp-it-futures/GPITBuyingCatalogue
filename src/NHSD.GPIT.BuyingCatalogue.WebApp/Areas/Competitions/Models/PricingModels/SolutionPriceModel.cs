using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class SolutionPriceModel
{
    public SolutionPriceModel(
        CompetitionSolution solution)
    {
        CatalogueItemId = solution.SolutionId;
        Name = solution.Solution.CatalogueItem.Name;

        var solutionMonthlyCost = solution?.Price?.CalculateCostPerMonth(0);
        var servicesMonthlyCost = solution.SolutionServices?.Sum(x => x.Price?.CalculateCostPerMonth(0));

        Price = solutionMonthlyCost + servicesMonthlyCost;
    }

    public CatalogueItemId CatalogueItemId { get; set; }

    public string Name { get; set; }

    public decimal? Price { get; set; }

    public TaskProgress Progress { get; set; } = TaskProgress.NotStarted;
}
