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
                .OrderBy(e => e.Capability)
                .ThenBy(e => e.IsActive)
                .ThenBy(e => e.Name)
                .ToList();
        }

        public IList<SupplierDefinedEpicModel> SupplierDefinedEpics { get; set; } = new List<SupplierDefinedEpicModel>();

        public bool ShowInactiveItems { get; }
    }
}
