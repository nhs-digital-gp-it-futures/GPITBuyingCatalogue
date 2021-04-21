using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class BrowserBasedModel
    { 
        public BrowserBasedModel()
        {
            ClientApplication = new ClientApplication();
        }

        public BrowserBasedModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            ClientApplication = catalogueItem.Solution.GetClientApplication();
        }

        public string SolutionId { get; set; }

        public ClientApplication ClientApplication { get; set; }

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
