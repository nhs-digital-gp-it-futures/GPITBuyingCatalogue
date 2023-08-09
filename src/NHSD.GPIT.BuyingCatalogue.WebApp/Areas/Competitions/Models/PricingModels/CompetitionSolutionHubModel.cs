using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
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
        SolutionId = competitionSolution.SolutionId;
        SolutionName = competitionSolution.Solution.CatalogueItem.Name;

        CatalogueItems = new[]
            {
                new CatalogueItemHubModel(
                    competitionSolution.SolutionId,
                    competitionSolution.Solution.CatalogueItem,
                    competitionSolution.Quantity,
                    competition.Recipients.ToDictionary(
                        x => x,
                        x => competitionSolution.Quantities.FirstOrDefault(y => y.OdsCode == x.Id)?.Quantity),
                    competitionSolution.Price)
                {
                    InternalOrgId = internalOrgId,
                    CompetitionId = competitionSolution.CompetitionId,
                    ContractLength = competition.ContractLength,
                },
            }.Union(
                competitionSolution.SolutionServices.Select(
                    x => new CatalogueItemHubModel(
                        competitionSolution.SolutionId,
                        x.Service,
                        x.Quantity,
                        competition.Recipients.ToDictionary(
                            y => y,
                            y => x.Quantities.FirstOrDefault(z => z.OdsCode == y.Id)?.Quantity),
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

    public List<CatalogueItemHubModel> CatalogueItems { get; set; }

    public CatalogueItemHubModel GetCatalogueItem(CatalogueItemId catalogueItemId) =>
        CatalogueItems.FirstOrDefault(x => x.CatalogueItemId == catalogueItemId);

    public IEnumerable<CatalogueItemHubModel> GetCatalogueItemsByType(CatalogueItemType catalogueItemType) =>
        CatalogueItems.Where(x => x.CatalogueItemType == catalogueItemType).OrderBy(x => x.CatalogueItemName);
}
