using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class SolutionModel : NavBaseModel
    {
        public SolutionModel()
        {
            BackLink = "/admin/catalogue-solutions";
        }

        public SolutionModel(CatalogueItem solution)
        {
            SupplierId = solution.SupplierId;
            SolutionName = solution.Name;
            SupplierName = solution.Supplier.Name;
            SolutionDisplayName = solution.Name;
        }

        public int? SupplierId { get; set; }

        public string SolutionName { get; set; }

        public string SolutionDisplayName { get; set; }

        public string SupplierName { get; set; }

        public IList<FrameworkModel> Frameworks { get; set; }

        public IEnumerable<SelectListItem> SuppliersSelectList { get; set; } = new List<SelectListItem>();

        public string Heading { get; set; }

        public string Description { get; set; }

        public SolutionModel WithSelectListItems(IList<Supplier> suppliers)
        {
            SuppliersSelectList = suppliers == null || !suppliers.Any()
            ? System.Array.Empty<SelectListItem>()
            : suppliers.Select(s => new SelectListItem($"{s.Name} ({s.Id})", s.Id.ToString(CultureInfo.InvariantCulture)));

            return this;
        }

        public SolutionModel WithAddSolution()
        {
            Heading = "Add a solution";
            Description = "Provide the following information about your Catalogue Solution.";
            return this;
        }

        public SolutionModel WithEditSolution()
        {
            Heading = "Details";
            Description = "These are the current details for this Catalogue Solution.";
            return this;
        }
    }
}
