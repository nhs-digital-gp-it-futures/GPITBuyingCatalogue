using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class SolutionDescriptionModel : MarketingBaseModel
    {
        public SolutionDescriptionModel() : base(null)
        {
        }

        public SolutionDescriptionModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                                    
            Summary = catalogueItem.Solution.Summary;
            Description = catalogueItem.Solution.FullDescription;
            Link = catalogueItem.Solution.AboutUrl;            
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Summary);

        [Required(ErrorMessage = "Enter a Summary")]
        [StringLength(350)]
        public string Summary { get; set; }

        [StringLength(1100)]
        public string Description { get; set; }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Give buyers as much detail as possible about your Catalogue Solution.",
                Title = "Catalogue Solution description",
            };
    }
}
