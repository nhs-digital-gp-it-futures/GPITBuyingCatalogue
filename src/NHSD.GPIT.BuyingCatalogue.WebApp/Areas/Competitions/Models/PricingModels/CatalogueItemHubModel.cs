using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class CatalogueItemHubModel
{
    public CatalogueItemHubModel()
    {
    }

    public CatalogueItemHubModel(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice)
    {
        SolutionId = solutionId;
        CatalogueItemName = catalogueItem.Name;
        CatalogueItemId = catalogueItem.Id;
        CatalogueItemType = catalogueItem.CatalogueItemType;
        NumberOfCataloguePrices = catalogueItem.CataloguePrices.Count;
        OdsOrganisations = recipientQuantities;
        Price = selectedPrice;
        PriceId = PriceProgress is TaskProgress.Completed
            ? Price.CataloguePriceId
            : catalogueItem.CataloguePrices.FirstOrDefault()?.CataloguePriceId;
        GlobalQuantity = globalQuantity;
        Quantity = (GlobalQuantity ?? OdsOrganisations.Sum(x => x.Value)).GetValueOrDefault();
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CatalogueItemName { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public CatalogueItemType CatalogueItemType { get; set; }

    public int NumberOfCataloguePrices { get; set; }

    public int? PriceId { get; set; }

    public int? GlobalQuantity { get; set; }

    public int? ContractLength { get; set; }

    public bool? AssociatedServicesAvailable { get; set; }

    public bool AssociatedServicesRemaining { get; set; }

    public IEnumerable<CatalogueItemHubModel> AssociatedServices { get; set; }

    public Dictionary<CompetitionSublocationRecipient, int?> OdsOrganisations { get; set; }

    public CompetitionCatalogueItemPrice Price { get; set; }

    public int Quantity { get; set; }

    public TaskProgress PriceProgress => Price?.Tiers?.Count > 0
        ? TaskProgress.Completed
        : TaskProgress.NotStarted;

    public TaskProgress QuantityProgress
    {
        get
        {
            if (PriceProgress is TaskProgress.NotStarted)
                return TaskProgress.CannotStart;

            return GlobalQuantity.HasValue || OdsOrganisations.All(x => x.Value.HasValue)
                ? TaskProgress.Completed
                : OdsOrganisations.All(x => !x.Value.HasValue)
                    ? TaskProgress.NotStarted
                    : TaskProgress.InProgress;
        }
    }

    public TaskProgress AssociatedServicesProgress
    {
        get
        {
            if (AssociatedServicesAvailable != true)
                return TaskProgress.NotApplicable;

            if (!AssociatedServices.Any())
                return TaskProgress.Optional;

            return AssociatedServices.Any(x =>
                x.PriceProgress == TaskProgress.NotStarted || x.QuantityProgress == TaskProgress.NotStarted)
                ? TaskProgress.InProgress
                : TaskProgress.Completed;
        }
    }

    public static string GetLinkName(TaskProgress progress) => progress switch
    {
        TaskProgress.NotStarted => "Start",
        TaskProgress.InProgress => "Continue",
        _ => "Change",
    };

    public string CalculateOneOffCost() =>
        ((IPrice)Price).CalculateOneOffCost(Quantity).ToString("N2");

    public string CalculateCostPerMonth() =>
        ((IPrice)Price).CalculateCostPerMonth(Quantity).ToString("N2");

    public string CalculateTotalCost() =>
        (((IPrice)Price).CalculateCostPerMonth(Quantity) * ContractLength.GetValueOrDefault()).ToString("N2");
}
