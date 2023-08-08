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
        string internalOrgId,
        CompetitionSolution competitionSolution,
        ICollection<CompetitionRecipient> competitionRecipients)
    {
        SolutionId = competitionSolution.SolutionId;
        SolutionName = competitionSolution.Solution.CatalogueItem.Name;

        CatalogueItems = new[]
            {
                new CatalogueItemHubModel(
                    competitionSolution.SolutionId,
                    competitionSolution.Solution.CatalogueItem,
                    competitionRecipients.ToDictionary(
                        y => y.OdsOrganisation,
                        y => y.GetRecipientQuantityForItem(competitionSolution.SolutionId)?.Quantity),
                    competitionSolution.Price)
                { InternalOrgId = internalOrgId, CompetitionId = competitionSolution.CompetitionId, },
            }.Union(
                competitionSolution.SolutionServices.Select(
                    x => new CatalogueItemHubModel(
                        competitionSolution.SolutionId,
                        x.Service,
                        competitionRecipients.ToDictionary(
                            y => y.OdsOrganisation,
                            y => y.GetRecipientQuantityForItem(x.ServiceId)?.Quantity),
                        x.Price) { InternalOrgId = internalOrgId, CompetitionId = competitionSolution.CompetitionId, }))
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
