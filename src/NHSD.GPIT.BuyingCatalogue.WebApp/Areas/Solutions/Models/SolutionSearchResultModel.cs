using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionSearchResultModel
    {
        public SolutionSearchResultModel(CatalogueItem catalogueItem, bool noLinks = false)
        {
            CatalogueItem = catalogueItem;
            NoLinks = noLinks;
        }

        public CatalogueItem CatalogueItem { get; init; }

        public bool NoLinks { get; init; }
    }
}
