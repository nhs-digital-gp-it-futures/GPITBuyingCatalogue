using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class HardwareRequirementsModel : MarketingBaseModel
    {
        public HardwareRequirementsModel() : base(null)
        {
        }

        public HardwareRequirementsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            Description = ClientApplication.HardwareRequirements;
        }

        public override bool? IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(ClientApplication.HardwareRequirements); }
        }     
        
        public string Description { get; set; }
    }
}
