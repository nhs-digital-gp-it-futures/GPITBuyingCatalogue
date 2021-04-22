using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class ConnectivityModel : NavBaseModel
    {
        public ConnectivityModel()
        {
        }

        public ConnectivityModel(CatalogueItem catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop";
            BackLinkText = "Return to all sections";


            SolutionId = catalogueItem.CatalogueItemId;
            ClientApplication = catalogueItem.Solution.GetClientApplication();
        }

        public string SolutionId { get; set; }

        public ClientApplication ClientApplication { get; set; }
    }
}
