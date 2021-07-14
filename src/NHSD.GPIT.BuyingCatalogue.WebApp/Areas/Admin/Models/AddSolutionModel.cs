using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class AddSolutionModel : NavBaseModel
    {
        public AddSolutionModel()
        {
            BackLink = "/admin/catalogue-solutions";
            BackLinkText = "Go back";
        }

        public string SupplierId { get; set; }

        public string SolutionName { get; set; }

        public string SupplierName { get; set; }

        public IList<FrameworkModel> Frameworks { get; set; }

        public IEnumerable<SelectListItem> SuppliersSelectList { get; set; } = new List<SelectListItem>();

        public AddSolutionModel WithSelectListItems(IList<Supplier> suppliers)
        {
            SuppliersSelectList = suppliers == null || !suppliers.Any()
            ? System.Array.Empty<SelectListItem>()
            : suppliers.Select(s => new SelectListItem($"{s.Name} ({s.Id})", s.Id));

            return this;
        }
    }
}
