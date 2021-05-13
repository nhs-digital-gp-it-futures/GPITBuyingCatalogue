using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
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

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            ThirdPartyComponents = ClientApplication?.NativeDesktopThirdParty?.ThirdPartyComponents;
            DeviceCapabilities = ClientApplication?.NativeDesktopThirdParty?.DeviceCapabilities;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopThirdParty?.ThirdPartyComponents) ||
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopThirdParty?.DeviceCapabilities);

        public string ThirdPartyComponents { get; set; }

        public string DeviceCapabilities { get; set; }
    }
}
