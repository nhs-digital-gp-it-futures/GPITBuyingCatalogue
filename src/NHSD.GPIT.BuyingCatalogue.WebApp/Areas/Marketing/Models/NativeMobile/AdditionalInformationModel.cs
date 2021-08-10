using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public sealed class AdditionalInformationModel : MarketingBaseModel
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

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.Id}/section/native-mobile";

            AdditionalInformation = ClientApplication?.NativeMobileAdditionalInformation;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeMobileAdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
