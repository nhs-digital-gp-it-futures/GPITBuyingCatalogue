using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public class SupplierDefinedEpicsDashboardModel
    {
        public SupplierDefinedEpicsDashboardModel(IEnumerable<Epic> supplierDefinedEpics, string searchTerm)
        {
            SupplierDefinedEpics = supplierDefinedEpics
                .Select(e => new SupplierDefinedEpicModel(e))
                .OrderBy(e => e.Capability)
                .ThenBy(e => e.IsActive)
                .ThenBy(e => e.Name)
                .ToList();

            SearchTerm = searchTerm;
        }

        public IList<SupplierDefinedEpicModel> SupplierDefinedEpics { get; }

        public string SearchTerm { get; }

        public bool ShowInactiveItems { get; }
    }
}
