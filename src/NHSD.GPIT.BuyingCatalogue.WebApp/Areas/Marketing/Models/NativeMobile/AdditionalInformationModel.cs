using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class AdditionalInformationModel : MarketingBaseModel
    {
        public AdditionalInformationModel() : base(null)
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }        
    }
}
