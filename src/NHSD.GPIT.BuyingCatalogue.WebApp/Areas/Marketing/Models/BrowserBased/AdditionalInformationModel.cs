using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class AdditionalInformationModel : NavBaseModel
    {
        public AdditionalInformationModel()
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based";
            BackLinkText = "Return to all sections";


            SolutionId = catalogueItem.CatalogueItemId;
            ClientApplication = catalogueItem.Solution.GetClientApplication();
        }

        public string SolutionId { get; set; }

        public ClientApplication ClientApplication { get; set; }
    }
}
