using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class MobileFirstApproachModel : MarketingBaseModel
    {
        public MobileFirstApproachModel()
            : base(null)
        {
        }

        public MobileFirstApproachModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            MobileFirstApproach = ClientApplication?.NativeMobileFirstDesign.ToYesNo();
        }

        public override bool? IsComplete => ClientApplication?.NativeMobileFirstDesign.HasValue;

        public string MobileFirstApproach { get; set; }

        public virtual bool? MobileFirstDesign() =>
            string.IsNullOrWhiteSpace(MobileFirstApproach)
                ? (bool?)null
                : MobileFirstApproach.Equals("Yes", StringComparison.InvariantCultureIgnoreCase);
    }
}
