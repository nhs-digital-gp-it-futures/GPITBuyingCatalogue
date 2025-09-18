using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

public sealed class AdditionalAssociatedServicesModel : AssociatedServicesModel
{
    public AdditionalAssociatedServicesModel(
        CatalogueItem catalogueItem,
        CatalogueItem additionalService,
        List<CatalogueItem> associatedServices,
        CatalogueItemContentStatus contentStatus)
        : base(catalogueItem, associatedServices, contentStatus, isSubPage: true)
    {
        Title = "Associated services";
        Caption = additionalService.Name;
        AdditionalServiceId = additionalService.Id;
    }

    public override int Index => 4;

    private CatalogueItemId AdditionalServiceId { get; }

    public override string GetPricePageUrl(IUrlHelper urlHelper, CatalogueItemId serviceId) => urlHelper.Action(
        nameof(SolutionsController.AssociatedServicePrice),
        typeof(SolutionsController).ControllerName(),
        new { solutionId = SolutionId, additionalServiceId = AdditionalServiceId, associatedServiceId = serviceId });
}
