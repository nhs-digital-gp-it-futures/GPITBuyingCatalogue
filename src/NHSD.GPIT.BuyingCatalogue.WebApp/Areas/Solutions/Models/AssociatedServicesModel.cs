using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AssociatedServicesModel : SolutionDisplayBaseModel
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
    }
}
