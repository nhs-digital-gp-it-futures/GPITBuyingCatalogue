using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class OperatingSystemsModel : MarketingBaseModel
    {
        public OperatingSystemsModel() : base(null)
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";                                    
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }        
    }
}
