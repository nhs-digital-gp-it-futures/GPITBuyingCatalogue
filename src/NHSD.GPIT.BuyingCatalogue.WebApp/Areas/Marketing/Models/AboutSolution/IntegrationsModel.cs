using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class IntegrationsModel : NavBaseModel
    {
        public IntegrationsModel()
        {
        }

        public IntegrationsModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            Link = catalogueItem.Solution.IntegrationsUrl;
        }

        public string SolutionId { get; set; }
        public string Link { get; set; }
    }
}
