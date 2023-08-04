using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class SolutionPriceModel
{
    public SolutionPriceModel(
        CompetitionSolution solution)
    {
        Name = solution.Solution.CatalogueItem.Name;
    }

    public string Name { get; set; }

    public double? Price { get; set; }

    public TaskProgress Progress { get; set; } = TaskProgress.NotStarted;
}
