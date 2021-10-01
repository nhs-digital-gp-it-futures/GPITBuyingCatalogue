using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AssociatedServicesModel : SolutionDisplayBaseModel
    {
        public AssociatedServicesModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            // TODO: make use of new nav property on catalogue item once merged
            Services = catalogueItem.Supplier.CatalogueItems
                .Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService)
                .OrderBy(c => c.Name)
                .Select(c => new AssociatedServiceModel(c.AssociatedService))
                .ToList();

            PaginationFooter.FullWidth = true;
        }

        public override int Index => 5;

        public IReadOnlyList<AssociatedServiceModel> Services { get; }

        public bool HasServices() => Services != null && Services.Any();
    }
}
