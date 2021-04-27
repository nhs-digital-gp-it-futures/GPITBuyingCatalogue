using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
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

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            if (ClientApplication?.NativeMobileFirstDesign != null)
                MobileFirstApproach = ClientApplication.NativeMobileFirstDesign.ToYesNo();
        }

        public override bool? IsComplete
        {
            get { return ClientApplication?.NativeMobileFirstDesign.HasValue; }
        }

        public string MobileFirstApproach { get; set; }
    }
}
