using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags.NhsTagsTagHelper;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ImplementationTimescaleModel : MarketingBaseModel
    {
        public ImplementationTimescaleModel()
            : base(null)
        {
        }

        public ImplementationTimescaleModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Description = CatalogueItem?.Solution?.ImplementationDetail;
            SolutionName = catalogueItem?.Name;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Description);

        [StringLength(1000)]
        public string Description { get; set; }

        public string SolutionName { get; set; }

        public string StatusImplementation() =>
            !string.IsNullOrWhiteSpace(Description)
                ? "Completed"
                : "Not started";

        public TagColour StatusImplementationColor() =>
            !string.IsNullOrWhiteSpace(Description)
                ? TagColour.Green
                : TagColour.Grey;
    }
}
