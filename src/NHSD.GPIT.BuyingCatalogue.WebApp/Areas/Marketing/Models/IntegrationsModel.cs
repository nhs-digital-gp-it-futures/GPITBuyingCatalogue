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
            Link = catalogueItem.Solution.IntegrationsUrl;
        }

        public string Id { get; set; }
        public string Link { get; set; }
    }
}
