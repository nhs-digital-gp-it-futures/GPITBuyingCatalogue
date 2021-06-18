using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class CatalogueSolutionsModel : NavBaseModel
    {
        public List<CatalogueItem> CatalogueItems { get; set; }
    }
}
