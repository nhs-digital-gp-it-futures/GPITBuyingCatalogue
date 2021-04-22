using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class MobileFirstApproachModel : MarketingBaseModel
    {
        public MobileFirstApproachModel() : base(null)
        {
        }

        public MobileFirstApproachModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            if (ClientApplication.MobileFirstDesign.HasValue)
                MobileFirstApproach = ClientApplication.MobileFirstDesign.GetValueOrDefault() ? "Yes" : "No";
        }

        public override bool? IsComplete
        {
            get { return ClientApplication.MobileFirstDesign; }
        }

        public string MobileFirstApproach { get; set; }
    }
}
