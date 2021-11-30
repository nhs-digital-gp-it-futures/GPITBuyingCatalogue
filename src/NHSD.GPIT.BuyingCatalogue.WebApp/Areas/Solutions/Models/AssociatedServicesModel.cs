using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AssociatedServicesModel : SolutionDisplayBaseModel
    {
        public AssociatedServicesModel(CatalogueItem catalogueItem, List<CatalogueItem> associatedServices)
            : base(catalogueItem)
        {
            Services = associatedServices;
            PaginationFooter.FullWidth = true;
        }

        public override int Index => 6;

        public IReadOnlyList<CatalogueItem> Services { get; }

        public bool HasServices() => Services != null && Services.Any();
    }
}
