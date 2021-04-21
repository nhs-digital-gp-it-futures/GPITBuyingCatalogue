using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using System.Linq;

// MJRTODO - Test this with a record without anything in it - null check...

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class BrowserBasedModel
    { 
        public BrowserBasedModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;

            if (!string.IsNullOrWhiteSpace(catalogueItem?.Solution?.ClientApplication))
                ClientApplication = JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            else
                ClientApplication = new ClientApplication();
        }

        public string SolutionId { get; set; }

        private ClientApplication ClientApplication { get; set; }

        public string SupportedBrowsersStatus
        {
            get { return ClientApplication.BrowsersSupported.Any() ? "COMPLETE" : "INCOMPLETE"; }
        }

        public string MobileFirstApproachStatus
        {
            // MJRTODO - Is this a 3 state checkbox in the current UI?
            get { return ClientApplication.MobileFirstDesign.HasValue ? "COMPLETE" : "INCOMPLETE"; }
        }

        public string PlugInsStatus
        {
            get { return "TODO"; }
        }

        public string ConnectivityStatus
        {
            get { return "TODO"; }
        }

        public string HardwareRequirementsStatus
        {
            get { return "TODO"; }
        }

        public string AdditionalInformationStatus
        {
            get { return "TODO"; }
        }
    }
}
