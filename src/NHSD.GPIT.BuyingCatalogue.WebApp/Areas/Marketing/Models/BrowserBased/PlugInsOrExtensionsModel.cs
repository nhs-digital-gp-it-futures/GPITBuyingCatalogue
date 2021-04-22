using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class PlugInsOrExtensionsModel : MarketingBaseModel
    {
        public PlugInsOrExtensionsModel() : base(null)
        {
        }

        public PlugInsOrExtensionsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            if (ClientApplication.Plugins?.Required != null)
            {
                PlugInsRequired = ClientApplication.Plugins.Required.Value ? "Yes" : "No";
                AdditionalInformation = ClientApplication.Plugins.AdditionalInformation;
            }
        }

        public override bool? IsComplete
        {
            get { return ClientApplication.Plugins?.Required; }
        }

        public string PlugInsRequired { get; set; }

        public string AdditionalInformation { get; set; }
    }
}
