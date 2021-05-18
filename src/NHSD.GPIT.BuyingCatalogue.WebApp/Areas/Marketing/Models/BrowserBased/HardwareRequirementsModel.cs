using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class HardwareRequirementsModel : MarketingBaseModel
    {
        public HardwareRequirementsModel()
            : base(null)
        {
        }

        public HardwareRequirementsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            Description = ClientApplication.HardwareRequirements;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(ClientApplication?.HardwareRequirements);

        [StringLength(500)]
        public string Description { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Let buyers know about any hardware requirements for your Catalogue Solution.",
                Title = "Browser-based application – hardware requirements",
            };
    }
}
