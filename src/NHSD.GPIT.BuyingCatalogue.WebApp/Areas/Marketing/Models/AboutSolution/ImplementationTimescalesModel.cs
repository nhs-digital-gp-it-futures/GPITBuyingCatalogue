using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class ImplementationTimescalesModel : MarketingBaseModel
    {
        public ImplementationTimescalesModel() : base(null)
        {
        }

        public ImplementationTimescalesModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            Description = CatalogueItem.Solution.ImplementationDetail;
        }

        protected override bool IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(CatalogueItem.Solution.ImplementationDetail); }
        }
        
        public string Description { get; set; }
    }
}
