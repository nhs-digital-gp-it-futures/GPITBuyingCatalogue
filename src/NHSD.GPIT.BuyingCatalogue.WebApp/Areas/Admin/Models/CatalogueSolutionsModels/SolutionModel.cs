using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels
{
    public sealed class SolutionModel : NavBaseModel
    {
        internal const string AddHeading = "Add a solution";
        internal const string EditHeading = "Details";

        internal const string AddDescription = "Provide the following information about your catalogue solution.";
        internal const string EditDescription = "These are the current details for this catalogue solution.";

        public SolutionModel()
        {
        }

        public SolutionModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.Id;
            SupplierId = catalogueItem.SupplierId;
            SolutionName = catalogueItem.Name;
            SolutionDisplayName = catalogueItem.Name;
            IsPilotSolution = catalogueItem.Solution.IsPilotSolution;
            SelectedCategory = catalogueItem.Solution.Category;
        }

        public CatalogueItemId? SolutionId { get; set; }

        public string SolutionName { get; set; }

        public string SolutionDisplayName { get; set; }

        public bool IsPilotSolution { get; set; }

        public int? SupplierId { get; set; }

        public SolutionCategory? SelectedCategory { get; set; }

        public IList<FrameworkModel> Frameworks { get; set; }

        public IEnumerable<SelectOption<string>> SuppliersSelectList { get; set; } = new List<SelectOption<string>>();

        public IEnumerable<SelectOption<SolutionCategory>> SolutionCategories => Enum.GetValues<SolutionCategory>()
            .Select(x => new SelectOption<SolutionCategory>(x.Description(), x));

        public string Heading => SolutionId is not null ? EditHeading : AddHeading;

        public string Description => SolutionId is not null ? EditDescription : AddDescription;

        public SolutionModel WithSelectListItems(IEnumerable<Supplier> suppliers)
        {
            SuppliersSelectList = suppliers
                .Select(s => new SelectOption<string>(
                    $"{s.Name} ({s.Id}) ({(s.IsActive ? "Active" : "Inactive")})",
                    $"{s.Id}"));

            return this;
        }
    }
}
