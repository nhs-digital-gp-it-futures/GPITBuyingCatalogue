using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class SolutionPriceModel
{
    public SolutionPriceModel(
        CompetitionSolution solution,
        Competition competition)
    {
        CatalogueItemId = solution.SolutionId;
        Name = solution.Solution.CatalogueItem.Name;

        Progress = solution.AllQuantitiesDefined(competition.Recipients) && solution.AllPricesDefined()
            ? TaskProgress.Completed
            : (solution.Price is not null || (solution.Quantity.HasValue || solution.Quantities.Any())) && (!solution.AllQuantitiesDefined(competition.Recipients) || solution.AllPricesDefined())
                ? TaskProgress.InProgress
                : TaskProgress.NotStarted;

        if (Progress is not TaskProgress.Completed) return;

        var solutionMonthlyCost =
            solution.Price?.CalculateCostPerMonth(solution.Quantity ?? solution.Quantities.Sum(x => x.Quantity));
        var servicesMonthlyCost = solution.SolutionServices?.Sum(
            x => x.Price?.CalculateCostPerMonth(x.Quantity ?? x.Quantities.Sum(y => y.Quantity)));
        var oneOffCost = solution.GetAssociatedServices()
            .Sum(x => x.Price?.CalculateOneOffCost(x.Quantity ?? x.Quantities.Sum(y => y.Quantity)));

        Price = oneOffCost + ((solutionMonthlyCost + servicesMonthlyCost) * competition.ContractLength);
    }

    public CatalogueItemId CatalogueItemId { get; }

    public string Name { get; }

    public decimal? Price { get; }

    public TaskProgress Progress { get; }
}
