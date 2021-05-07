using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class IntegrationsModel : MarketingBaseModel
    {
        public IntegrationsModel() : base(null)
        {
        }

        public IntegrationsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            Link = CatalogueItem.Solution.IntegrationsUrl;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(CatalogueItem?.Solution?.IntegrationsUrl);

        public string Link { get; set; }
    }
}
