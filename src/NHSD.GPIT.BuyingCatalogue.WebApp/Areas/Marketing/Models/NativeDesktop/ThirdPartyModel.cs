using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

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

        [StringLength(500)]
        public string ThirdPartyComponents { get; set; }

        [StringLength(500)]
        public string DeviceCapabilities { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice =
                    "Let buyers know about any third-party components or device capabilities needed for your Catalogue Solution.",
                Title = "Native desktop application – third-party components and device capabilities",
            };
    }
}
