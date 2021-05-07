using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

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

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(CatalogueItem?.Solution?.Summary);

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }      
    }
}
