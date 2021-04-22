using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;

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

        public override bool? IsComplete
        {
            get 
            { 
                return new SupportedBrowsersModel(CatalogueItem).IsComplete.GetValueOrDefault() &&
                    new MobileFirstApproachModel(CatalogueItem).IsComplete.GetValueOrDefault() &&
                    new PlugInsOrExtensionsModel(CatalogueItem).IsComplete.GetValueOrDefault() &&
                    new ConnectivityAndResolutionModel(CatalogueItem).IsComplete.GetValueOrDefault();                
            }
        }
        
        public string SupportedBrowsersStatus
        {
            get { return GetStatus(new SupportedBrowsersModel(CatalogueItem)); }
        }

        public string MobileFirstApproachStatus
        {            
            get { return GetStatus(new MobileFirstApproachModel(CatalogueItem)); }
        }

        public string PlugInsStatus
        {
            get { return GetStatus(new PlugInsOrExtensionsModel(CatalogueItem)); }
        }

        public string ConnectivityStatus
        {
            get { return GetStatus(new ConnectivityAndResolutionModel(CatalogueItem)); }
        }

        public string HardwareRequirementsStatus
        {
            get { return GetStatus(new HardwareRequirementsModel(CatalogueItem)); }
        }

        public string AdditionalInformationStatus
        {
            get { return GetStatus(new AdditionalInformationModel(CatalogueItem)); }
        }
    }
}
