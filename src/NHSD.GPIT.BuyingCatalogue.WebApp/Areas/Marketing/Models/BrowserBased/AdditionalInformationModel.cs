using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
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

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            AdditionalInformation = ClientApplication.AdditionalInformation;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(ClientApplication?.AdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Let buyers know any additional information about your Catalogue Solution.",
                Title = "Browser-based application – additional information",
            };
    }
}
