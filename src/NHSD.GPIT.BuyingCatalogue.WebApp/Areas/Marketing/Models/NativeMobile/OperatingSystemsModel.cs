using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class OperatingSystemsModel : MarketingBaseModel
    {
        public OperatingSystemsModel() : base(null)
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";
        }

        public override bool? IsComplete
        {
            get { return ClientApplication.MobileOperatingSystems?.OperatingSystems?.Any(); }
        }        
    }
}
