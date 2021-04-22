using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation
{
    public class AboutSupplierModel : MarketingBaseModel
    { 
        public AboutSupplierModel() : base(null)
        {
        }

        public AboutSupplierModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                                 
            Description = CatalogueItem.Supplier.Summary;
            Link = CatalogueItem.Supplier.SupplierUrl;
        }

        protected override bool IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(CatalogueItem.Supplier.SupplierUrl) || string.IsNullOrWhiteSpace(CatalogueItem.Supplier.Summary); }
        }
                
        public string Description { get; set; }

        public string Link { get; set; }               
    }
}
