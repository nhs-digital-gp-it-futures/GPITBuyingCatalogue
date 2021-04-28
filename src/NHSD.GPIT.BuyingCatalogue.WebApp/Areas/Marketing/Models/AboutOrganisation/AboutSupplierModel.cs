using System;
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
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                                 
            Description = CatalogueItem.Supplier.Summary;
            Link = CatalogueItem.Supplier.SupplierUrl;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(CatalogueItem?.Supplier?.SupplierUrl) ||
                                            !string.IsNullOrWhiteSpace(CatalogueItem?.Supplier?.Summary);

        public string Description { get; set; }

        public string Link { get; set; }               
    }
}
