using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class BrowserBasedModel : MarketingBaseModel
    { 
        public BrowserBasedModel() : base(null)
        {
            ClientApplication = new ClientApplication();
        }

        public BrowserBasedModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }
        
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
