using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices
{
    public sealed class AssociatedServicesModel : NavBaseModel
    {
        public AssociatedServicesModel()
        {
        }

        public AssociatedServicesModel(Supplier supplier, IEnumerable<AssociatedService> associatedServices)
        {
            SupplierId = supplier.Id;
            SupplierName = supplier.Name;

            AssociatedServices = associatedServices;
        }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public SolutionType? SolutionType { get; set; }

        public CatalogueItemId? SolutionId { get; set; }

        public CatalogueItemId? AdditionalServiceId { get; set; }

        public IEnumerable<AssociatedService> AssociatedServices { get; } = [];
    }
}
