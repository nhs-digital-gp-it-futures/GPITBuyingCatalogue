using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels
{
    public sealed class SolutionModel : NavBaseModel
    {
        public SolutionModel()
        {
        }

        public SolutionModel(CatalogueItem catalogueItem)
        {
            SolutionIdDisplay = catalogueItem.Id.ToString();
            SupplierId = catalogueItem.SupplierId;
            SupplierDisplayName = $"{catalogueItem.Supplier.Name} ({catalogueItem.SupplierId})";
            SolutionName = catalogueItem.Name;
            SolutionDisplayName = catalogueItem.Name;
            IsPilotSolution = catalogueItem.Solution.IsPilotSolution;
            IsEdit = true;
        }

        public bool IsEdit { get; set; }

        public string SolutionIdDisplay { get; set; }

        public CatalogueItemId? SolutionId
        {
            get
            {
                try
                {
                    return CatalogueItemId.ParseExact(SolutionIdDisplay);
                }
                catch
                {
                    return default;
                }
            }
        }

        public string SolutionName { get; set; }

        public string SolutionDisplayName { get; set; }

        public bool IsPilotSolution { get; set; }

        public int? SupplierId { get; set; }

        public string SupplierDisplayName { get; set; }

        public IList<FrameworkModel> Frameworks { get; set; }

        public IEnumerable<SelectOption<string>> SuppliersSelectList { get; set; } = new List<SelectOption<string>>();

        public string Heading { get; set; }

        public string Description { get; set; }

        public SolutionModel WithSelectListItems(IList<Supplier> suppliers)
        {
            SuppliersSelectList = suppliers == null || !suppliers.Any()
                ? Enumerable.Empty<SelectOption<string>>()
                : suppliers.Select(s => new SelectOption<string>($"{s.Name} ({s.Id})", $"{s.Id}"));

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
