using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class AssociatedServicesModel : NavBaseModel
    {
        public AssociatedServicesModel()
        {
        }

        public AssociatedServicesModel(CatalogueItem catalogueItem, IReadOnlyList<CatalogueItem> associatedServices)
        {
            Solution = catalogueItem;
            SelectableAssociatedServices = associatedServices.Select(s => new SelectableAssociatedService
            {
                Name = s.Name,
                Description = s.AssociatedService.Description,
                PublishedStatus = s.PublishedStatus,
                CatalogueItemId = s.AssociatedService.CatalogueItemId,
                Selected = catalogueItem.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == s.Id),
            }).ToList();
        }

        public CatalogueItem Solution { get; }

        public List<SelectableAssociatedService> SelectableAssociatedServices { get; } = new();
    }
}
