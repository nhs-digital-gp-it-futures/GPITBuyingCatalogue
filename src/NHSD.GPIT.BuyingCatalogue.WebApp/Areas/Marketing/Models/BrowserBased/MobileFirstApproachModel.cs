using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class MobileFirstApproachModel : MarketingBaseModel
    {
        public MobileFirstApproachModel() : base(null)
        {
        }

        public MobileFirstApproachModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            if (ClientApplication.MobileFirstDesign.HasValue)
                MobileFirstApproach = ClientApplication.MobileFirstDesign.ToYesNo();
        }

        public override bool? IsComplete
        {
            get { return ClientApplication?.MobileFirstDesign.HasValue; }
        }

        public string MobileFirstApproach { get; set; }
    }
}
