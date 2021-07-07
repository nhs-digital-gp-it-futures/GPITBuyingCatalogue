using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class ThirdPartyModel : MarketingBaseModel
    {
        public ThirdPartyModel()
            : base(null)
        {
        }

        public ThirdPartyModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
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

        [StringLength(500)]
        public string ThirdPartyComponents { get; set; }

        [StringLength(500)]
        public string DeviceCapabilities { get; set; }
    }
}
