using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class IntegrationsModel
    {
        public IntegrationsModel()
        {
        }

        public IntegrationsModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            Link = catalogueItem.Solution.IntegrationsUrl;
        }

        public string SolutionId { get; set; }
        public string Link { get; set; }
    }
}
