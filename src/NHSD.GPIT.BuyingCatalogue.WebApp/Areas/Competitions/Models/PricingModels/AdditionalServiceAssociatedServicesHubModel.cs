using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class AdditionalServiceAssociatedServicesHubModel : NavBaseModel
{
    [Required]
    public string InternalOrgId { get; set; }

    [Required]
    public int CompetitionId { get; set; }

    [Required]
    public CatalogueItemId SolutionId { get; set; }

    [Required]
    public CatalogueItemId AdditionalServiceItemId { get; init; }

    [Required]
    public string AdditionalServiceName { get; init; }

    [Required]
    public IEnumerable<AdditionalServiceAssociatedServiceItemModel> AssociatedServices { get; init; }

    [Required]
    public bool AssociatedServicesRemaining { get; set; }
}
