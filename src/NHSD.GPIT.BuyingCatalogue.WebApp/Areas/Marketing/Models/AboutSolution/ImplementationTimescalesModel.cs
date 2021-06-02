using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class ImplementationTimescalesModel : MarketingBaseModel
    {
        public ImplementationTimescalesModel()
            : base(null)
        {
        }

        public ImplementationTimescalesModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
            Description = CatalogueItem.Solution.ImplementationDetail;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Description);

        [StringLength(1100)]
        public string Description { get; set; }
    }
}
