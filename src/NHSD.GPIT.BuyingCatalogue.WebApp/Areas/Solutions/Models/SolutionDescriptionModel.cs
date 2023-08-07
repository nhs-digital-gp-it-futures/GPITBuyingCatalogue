using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionDescriptionModel : SolutionDisplayBaseModel
    {
        public SolutionDescriptionModel()
        {
        }

        public SolutionDescriptionModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            var solution = catalogueItem.Solution;

            Description = solution.FullDescription;
            Summary = solution.Summary;
            AboutUrl = catalogueItem.Solution.AboutUrl;
        }

        public string Description { get; init; }

        public string AboutUrl { get; init; }

        public override int Index => 0;

        public string Summary { get; init; }

        public bool HasDescription() => !string.IsNullOrWhiteSpace(Description);

        public bool HasSummary() => !string.IsNullOrWhiteSpace(Summary);

        public bool HasAboutUrl() => !string.IsNullOrWhiteSpace(AboutUrl);
    }
}
