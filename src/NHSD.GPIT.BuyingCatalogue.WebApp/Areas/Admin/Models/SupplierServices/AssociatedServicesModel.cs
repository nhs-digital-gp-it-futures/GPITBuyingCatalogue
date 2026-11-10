using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

        public required SolutionType SolutionType { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public IEnumerable<AssociatedService> AssociatedServices { get; } = [];
    }
}
