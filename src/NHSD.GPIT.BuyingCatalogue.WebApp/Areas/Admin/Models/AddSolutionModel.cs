using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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

        public IDictionary<string, string> Suppliers { get; set; } = new Dictionary<string, string>();

        public FrameworkModel FrameworkModel { get; set; }

        public IEnumerable<SelectListItem> SuppliersSelectList => Suppliers == null || !Suppliers.Any()
            ? System.Array.Empty<SelectListItem>()
            : Suppliers.Select(s => new SelectListItem($"{s.Value} ({s.Key})", s.Key));

        public AddSolutionModel WithSuppliers(IList<Supplier> suppliers)
        {
            Suppliers = suppliers?.ToDictionary(x => x.Id, x => x.Name);

            return this;
        }
    }
}
