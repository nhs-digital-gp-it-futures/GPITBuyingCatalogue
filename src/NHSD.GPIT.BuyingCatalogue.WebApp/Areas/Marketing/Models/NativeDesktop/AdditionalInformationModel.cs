using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class AdditionalInformationModel : MarketingBaseModel
    {
        public AdditionalInformationModel()
            : base(null)
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            AdditionalInformation = ClientApplication?.NativeDesktopAdditionalInformation;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopAdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
