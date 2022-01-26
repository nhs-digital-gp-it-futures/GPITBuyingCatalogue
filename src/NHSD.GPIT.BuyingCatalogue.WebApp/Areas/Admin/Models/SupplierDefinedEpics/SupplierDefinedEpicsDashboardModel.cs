using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public class SupplierDefinedEpicsDashboardModel
    {
        public SupplierDefinedEpicsDashboardModel(List<Epic> supplierDefinedEpics)
        {
            SupplierDefinedEpics = supplierDefinedEpics
                .Select(e => new SupplierDefinedEpicModel(e.Id, e.Name, e.Capability.Name, e.IsActive))
                .ToList();
        }

        public IList<SupplierDefinedEpicModel> SupplierDefinedEpics { get; set; } = new List<SupplierDefinedEpicModel>();
    }
}
