using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class PlugInsOrExtensionsModel : MarketingBaseModel
    {
        public PlugInsOrExtensionsModel() : base(null)
        {
        }

        public PlugInsOrExtensionsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            if (ClientApplication.Plugins?.Required != null)
            {
                PlugInsRequired = ClientApplication.Plugins.Required.ToYesNo();
                AdditionalInformation = ClientApplication.Plugins.AdditionalInformation;
            }
        }

        public override bool? IsComplete
        {
            get { return ClientApplication?.Plugins?.Required.HasValue; }
        }

        public string PlugInsRequired { get; set; }

        public string AdditionalInformation { get; set; }
    }
}
