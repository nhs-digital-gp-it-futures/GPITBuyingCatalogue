using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

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

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice =
                    "Describe the typical processes and timescales to implement your Catalogue Solution. Include typical timescales that maybe required to transition from an existing solution if applicable.",
                Title = "Catalogue Solution implementation timescales",
            };
    }
}
