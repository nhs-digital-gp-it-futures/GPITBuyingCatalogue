using System;
using System.ComponentModel.DataAnnotations;
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

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Description) ||
                                            !string.IsNullOrWhiteSpace(Link);

        [StringLength(1100)]
        public string Description { get; set; }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }               
    }
}
