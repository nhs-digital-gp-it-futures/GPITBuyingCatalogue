using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public SolutionsModel()
        {
        }

        public SolutionsModel(Dictionary<EntityFramework.Catalogue.Models.Framework, int> frameworks)
        {
            FrameworkFilters = frameworks
               .Select(f =>
               new FrameworkFilter
               {
                   FrameworkId = f.Key.Id,
                   FrameworkFullName = $"{f.Key.ShortName} {(f.Key.Id == "All" ? "frameworks" : "framework")}",
                   Count = f.Value,
               }).ToList();
        }

        public IList<CatalogueItem> CatalogueItems { get; set; }

        public IList<FrameworkFilter> FrameworkFilters { get; set; } = new List<FrameworkFilter>();

        public PageOptions Options { get; set; }

        public string SelectedFramework { get; set; }
    }
}
