using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public List<CatalogueItem> CatalogueItems { get; set; }
    }
}
