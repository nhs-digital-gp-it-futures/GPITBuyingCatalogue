using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
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

        var competitionSolutionProgress = new CompetitionSolutionProgress(solution, competition.Recipients);

        Progress = competitionSolutionProgress.Progress;

        if (Progress is not TaskProgress.Completed) return;

        Price = solution.CalculateTotalPrice(competition.ContractLength.GetValueOrDefault());
    }

    public CatalogueItemId CatalogueItemId { get; }

    public string Name { get; }

    public decimal? Price { get; }

    public TaskProgress Progress { get; }
}
