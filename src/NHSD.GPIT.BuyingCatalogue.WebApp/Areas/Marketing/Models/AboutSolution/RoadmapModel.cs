using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class RoadmapModel : MarketingBaseModel
    {
        public RoadmapModel() : base(null)
        {
        }

        public RoadmapModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            Summary = CatalogueItem.Solution.RoadMap;
        }

        public override bool? IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(CatalogueItem?.Solution?.RoadMap); }
        }

        public string Summary { get; set; }
    }
}
