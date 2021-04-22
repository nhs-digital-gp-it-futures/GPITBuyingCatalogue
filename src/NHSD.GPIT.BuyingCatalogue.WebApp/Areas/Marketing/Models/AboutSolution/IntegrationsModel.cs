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
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            Link = CatalogueItem.Solution.IntegrationsUrl;
        }

        public override bool? IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(CatalogueItem.Solution?.IntegrationsUrl); }
        }
        
        public string Link { get; set; }
    }
}
