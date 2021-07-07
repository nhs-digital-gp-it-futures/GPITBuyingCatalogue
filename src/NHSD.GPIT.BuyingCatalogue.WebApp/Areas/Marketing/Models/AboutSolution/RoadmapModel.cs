using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class RoadmapModel : MarketingBaseModel
    {
        public RoadmapModel()
            : base(null)
        {
        }

        public RoadmapModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
            Summary = CatalogueItem.Solution.RoadMap;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Summary);

        [StringLength(1000)]
        public string Summary { get; set; }
    }
}
