using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class CompetitionSolutionHubModel : NavBaseModel
{
    public CompetitionSolutionHubModel()
    {
    }

    public CompetitionSolutionHubModel(
        CompetitionSolution competitionSolution,
        ICollection<CompetitionRecipient> competitionRecipients)
    {
        SolutionId = competitionSolution.SolutionId;
        SolutionName = competitionSolution.Solution.CatalogueItem.Name;

        var catalogueItems = competitionSolution.SolutionServices.Select(x => x.Service);

        CatalogueItems = catalogueItems.Concat(new[] { competitionSolution.Solution.CatalogueItem })
            .Select(
                x => new CatalogueItemHubModel(
                    x,
                    competitionRecipients.ToDictionary(
                        y => y.OdsOrganisation,
                        y => y.GetRecipientQuantityForItem(x.Id)?.Quantity)))
            .ToList();
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public List<CatalogueItemHubModel> CatalogueItems { get; set; }

    public CatalogueItemHubModel GetCatalogueItem(CatalogueItemId catalogueItemId) =>
        CatalogueItems.FirstOrDefault(x => x.CatalogueItemId == catalogueItemId);

    public IEnumerable<CatalogueItemHubModel> GetCatalogueItemsByType(CatalogueItemType catalogueItemType) =>
        CatalogueItems.Where(x => x.CatalogueItemType == catalogueItemType).OrderBy(x => x.CatalogueItemName);
}
