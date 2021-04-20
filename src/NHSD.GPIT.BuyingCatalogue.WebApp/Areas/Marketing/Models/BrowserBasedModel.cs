using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class BrowserBasedModel
    { 
        public BrowserBasedModel(CatalogueItem catalogueItem)
        {
            Id = catalogueItem.CatalogueItemId;
            ClientApplication = JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
        }

        public string Id { get; set; }

        private ClientApplication ClientApplication { get; set; }

        public string SupportedBrowsersStatus
        {
            get { return "TODO"; }
        }

        public string MobileFirstApproachStatus
        {
            get { return "TODO"; }
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
