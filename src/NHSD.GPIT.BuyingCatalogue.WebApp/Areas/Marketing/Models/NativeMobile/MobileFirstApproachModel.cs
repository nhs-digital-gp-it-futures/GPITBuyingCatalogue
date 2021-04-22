using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class MobileFirstApproachModel : MarketingBaseModel
    {
        public MobileFirstApproachModel() : base(null)
        {
        }

        public MobileFirstApproachModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            if (ClientApplication.NativeMobileFirstDesign.HasValue)
                MobileFirstApproach = ClientApplication.NativeMobileFirstDesign.GetValueOrDefault() ? "Yes" : "No";
        }

        public override bool? IsComplete
        {
            get { return ClientApplication.NativeMobileFirstDesign; }
        }

        public string MobileFirstApproach { get; set; }
    }
}
