using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AssociatedServicesModel : SolutionDisplayBaseModel
    {
        public AssociatedServicesModel(
            CatalogueItem catalogueItem,
            List<CatalogueItem> associatedServices,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            Services = associatedServices;
            PaginationFooter.FullWidth = true;
        }

        public override int Index => 5;

        public IReadOnlyList<CatalogueItem> Services { get; }

        public bool HasServices() => Services != null && Services.Any();

        public virtual string GetPricePageUrl(IUrlHelper urlHelper, CatalogueItemId serviceId) => urlHelper.Action(
            nameof(SolutionsController.AssociatedServicePrice),
            typeof(SolutionsController).ControllerName(),
            new { solutionId = SolutionId, serviceId });
    }
}
