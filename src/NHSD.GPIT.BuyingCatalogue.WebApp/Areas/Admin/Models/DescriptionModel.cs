using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags.NhsTagsTagHelper;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class DescriptionModel : MarketingBaseModel
    {
        public DescriptionModel()
            : base(null)
        {
        }

        public DescriptionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Summary = catalogueItem.Solution?.Summary;
            Description = catalogueItem.Solution?.FullDescription;
            Link = catalogueItem.Solution.AboutUrl;
            SolutionName = catalogueItem?.Name;
        }

        public string SolutionName { get; set; }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Summary);

        [Required(ErrorMessage = "Enter a summary")]
        [StringLength(350)]
        public string Summary { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public string StatusDescription() =>
            IsComplete == true
                ? "Completed"
                : "Not started";

        public TagColour StatusDescriptionColor() =>
            IsComplete == true
                ? TagColour.Green
                : TagColour.Grey;
    }
}
