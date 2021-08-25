using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public IList<CatalogueItem> CatalogueItems { get; set; }

        public PageOptions Options { get; set; }
    }
}
