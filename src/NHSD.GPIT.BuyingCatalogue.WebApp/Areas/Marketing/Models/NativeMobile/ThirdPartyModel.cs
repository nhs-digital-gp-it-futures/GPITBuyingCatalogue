using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class ThirdPartyModel : MarketingBaseModel
    {
        public ThirdPartyModel() : base(null)
        {
        }

        public ThirdPartyModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            ThirdPartyComponents = ClientApplication?.MobileThirdParty?.ThirdPartyComponents;
            DeviceCapabilities = ClientApplication?.MobileThirdParty?.DeviceCapabilities;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.MobileThirdParty?.ThirdPartyComponents) ||
            !string.IsNullOrWhiteSpace(ClientApplication?.MobileThirdParty?.DeviceCapabilities);

        public string ThirdPartyComponents { get; set; }

        public string DeviceCapabilities { get; set; }
    }
}
