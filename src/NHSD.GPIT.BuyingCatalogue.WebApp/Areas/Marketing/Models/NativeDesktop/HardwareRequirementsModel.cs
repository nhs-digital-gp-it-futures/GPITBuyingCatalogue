using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class HardwareRequirementsModel : MarketingBaseModel
    {
        public HardwareRequirementsModel() : base(null)
        {
        }

        public HardwareRequirementsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            Description = ClientApplication.NativeDesktopHardwareRequirements;
        }

        public override bool? IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopHardwareRequirements); }
        }

        public string Description { get; set; }
    }
}
