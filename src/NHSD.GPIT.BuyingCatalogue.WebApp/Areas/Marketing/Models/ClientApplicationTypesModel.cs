using System;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class ClientApplicationTypesModel
    {
        public ClientApplicationTypesModel()
        {
        }

        public ClientApplicationTypesModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;

            var clientApplication = new ClientApplication();

            if (!string.IsNullOrWhiteSpace(catalogueItem?.Solution?.ClientApplication))
                clientApplication = JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
                                        
            BrowserBased = clientApplication.ClientApplicationTypes.Any(x => x.Equals("browser-based", StringComparison.InvariantCultureIgnoreCase));
            NativeMobile = clientApplication.ClientApplicationTypes.Any(x => x.Equals("native-mobile", StringComparison.InvariantCultureIgnoreCase));
            NativeDesktop = clientApplication.ClientApplicationTypes.Any(x => x.Equals("native-desktop", StringComparison.InvariantCultureIgnoreCase));
        }

        public string SolutionId { get; set; }

        public bool BrowserBased { get; set; }

        public bool NativeMobile { get; set; }

        public bool NativeDesktop { get; set; }
    }
}
