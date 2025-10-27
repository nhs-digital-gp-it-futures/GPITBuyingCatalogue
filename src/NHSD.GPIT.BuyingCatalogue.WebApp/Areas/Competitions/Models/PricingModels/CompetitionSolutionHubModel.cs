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
        Competition competition)
    {
        SolutionId = competitionSolution.CatalogueItemId;
        SolutionName = competitionSolution.CatalogueItem.Name;

        CatalogueItems = new[]
            {
                new CatalogueItemHubModel(
                    competitionSolution.CatalogueItemId,
                    competitionSolution.CatalogueItem,
                    competitionSolution.Quantity,
                    competition.FlattenedRecipients.ToDictionary(
                        x => x,
                        x => competitionSolution.Quantities
                            .FirstOrDefault(y => y.RecipientOdsCode == x.RecipientOdsCode)
                            ?.Quantity),
                    competitionSolution.Price)
                {
                    InternalOrgId = internalOrgId,
                    CompetitionId = competitionSolution.CompetitionId,
                    ContractLength = competition.ContractLength,
                },
            }.Union(
                competitionSolution.Services.Select(x => new CatalogueItemHubModel(
                    competitionSolution.CatalogueItemId,
                    x.CatalogueItem,
                    x.Quantity,
                    competition.FlattenedRecipients.ToDictionary(
                        y => y,
                        y => x.Quantities.FirstOrDefault(z => z.RecipientOdsCode == y.RecipientOdsCode)?.Quantity),
                    x.Price)
                {
                    InternalOrgId = internalOrgId,
                    CompetitionId = competitionSolution.CompetitionId,
                    ContractLength = competition.ContractLength,
                }))
            .ToList();
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public bool AssociatedServicesAvailable { get; set; }

    public bool AssociatedServicesRemaining { get; set; }

    public string AssociatedServicesUrl { get; set; }

    public List<CatalogueItemHubModel> CatalogueItems { get; set; }

    public IEnumerable<CatalogueItemHubModel> AssociatedServices =>
        GetCatalogueItemsByType(CatalogueItemType.AssociatedService);

    public CatalogueItemHubModel GetCatalogueItem(CatalogueItemId catalogueItemId) =>
        CatalogueItems.FirstOrDefault(x => x.CatalogueItemId == catalogueItemId);

    public IEnumerable<CatalogueItemHubModel> GetCatalogueItemsByType(CatalogueItemType catalogueItemType) =>
        CatalogueItems.Where(x => x.CatalogueItemType == catalogueItemType).OrderBy(x => x.CatalogueItemName);
}
