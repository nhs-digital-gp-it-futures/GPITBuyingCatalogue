using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class RoadmapModel
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public RoadmapModel FromCatalogueItem(CatalogueItem catalogueItem)
        {
            catalogueItem.ValidateNotNull(nameof(catalogueItem));

            Link = catalogueItem.Solution?.RoadMap;
            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;

            return this;
        }

        public TaskProgress Status() =>
            string.IsNullOrWhiteSpace(Link)
                ? TaskProgress.Optional
                : TaskProgress.Completed;
    }
}
