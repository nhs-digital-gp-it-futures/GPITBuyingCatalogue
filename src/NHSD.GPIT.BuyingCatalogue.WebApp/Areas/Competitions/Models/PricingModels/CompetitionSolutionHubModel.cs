using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class CompetitionSolutionHubModel : NavBaseModel
{
    public CompetitionSolutionHubModel(
        string internalOrgId,
        CompetitionSolution competitionSolution,
        IEnumerable<CompetitionSublocationRecipient> flattenedRecipients,
        int? contractLength)
    {
        SolutionId = competitionSolution.CatalogueItemId;
        SolutionName = competitionSolution.CatalogueItem.Name;
        SolutionItem = new CatalogueItemHubModel(
            competitionSolution.CatalogueItemId,
            competitionSolution.CatalogueItem,
            competitionSolution.Quantity,
            flattenedRecipients.ToDictionary(
                x => x,
                x => competitionSolution.Quantities
                    .FirstOrDefault(y => y.RecipientOdsCode == x.RecipientOdsCode)
                    ?.Quantity),
            competitionSolution.Price)
        {
            InternalOrgId = internalOrgId,
            CompetitionId = competitionSolution.CompetitionId,
            ContractLength = contractLength,
            AssociatedServicesAvailable = competitionSolution.AssociatedServicesAvailable,
            AssociatedServicesRemaining = competitionSolution.AssociatedServicesRemaining,
            AssociatedServices = competitionSolution.Services
                .Where(x => x.CatalogueItemType == CatalogueItemType.AssociatedService)
                .Select(x => new CatalogueItemHubModel(
                    competitionSolution.CatalogueItemId,
                    x.CatalogueItem,
                    x.Quantity,
                    flattenedRecipients.ToDictionary(
                        y => y,
                        y => x.Quantities.FirstOrDefault(z => z.RecipientOdsCode == y.RecipientOdsCode)?.Quantity),
                    x.Price)
                {
                    InternalOrgId = internalOrgId,
                    CompetitionId = competitionSolution.CompetitionId,
                    ContractLength = contractLength,
                }),
        };

        AdditionalServices = competitionSolution.GetAdditionalServices()
            .Select(x => new CatalogueItemHubModel(
                competitionSolution.CatalogueItemId,
                x.CatalogueItem,
                x.Quantity,
                flattenedRecipients.ToDictionary(
                    y => y,
                    y => x.Quantities.FirstOrDefault(z => z.RecipientOdsCode == y.RecipientOdsCode)?.Quantity),
                x.Price)
            {
                InternalOrgId = internalOrgId,
                CompetitionId = competitionSolution.CompetitionId,
                ContractLength = contractLength,
                AssociatedServicesAvailable = x.AssociatedServicesAvailable,
                AssociatedServices = x.AssociatedServices.Select(s => new CatalogueItemHubModel(
                    competitionSolution.CatalogueItemId,
                    s.CatalogueItem,
                    s.Quantity,
                    flattenedRecipients.ToDictionary(
                        y => y,
                        y => s.Quantities.FirstOrDefault(z => z.RecipientOdsCode == y.RecipientOdsCode)?.Quantity),
                    s.Price)
                {
                    InternalOrgId = internalOrgId,
                    CompetitionId = competitionSolution.CompetitionId,
                    ContractLength = contractLength,
                }),
            });
    }

    public CatalogueItemId SolutionId { get; }

    public string SolutionName { get; }

    public string AssociatedServicesUrl { get; set; }

    public CatalogueItemHubModel SolutionItem { get; set; }

    public IEnumerable<CatalogueItemHubModel> AdditionalServices { get; set; }
}
