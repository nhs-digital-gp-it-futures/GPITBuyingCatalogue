using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class AdditionalInformationModel : MarketingBaseModel
    {
        public AdditionalInformationModel() : base(null)
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            AdditionalInformation = ClientApplication.AdditionalInformation;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(ClientApplication?.AdditionalInformation);

        public string AdditionalInformation { get; set; }
    }
}
