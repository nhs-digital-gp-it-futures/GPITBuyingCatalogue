using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class HardwareRequirementsModel : MarketingBaseModel
    {
        public HardwareRequirementsModel() : base(null)
        {
        }

        public HardwareRequirementsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            Description = ClientApplication.NativeMobileHardwareRequirements;
        }

        public override bool? IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(ClientApplication.NativeMobileHardwareRequirements); }
        }

        public string Description { get; set; }
    }
}
