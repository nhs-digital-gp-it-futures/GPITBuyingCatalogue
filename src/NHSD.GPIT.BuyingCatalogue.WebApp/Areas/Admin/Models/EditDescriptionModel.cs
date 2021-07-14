using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class EditDescriptionModel : MarketingBaseModel
    {
        public EditDescriptionModel()
            : base(null)
        {
        }

        public EditDescriptionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLinkText = "Go back";
            Summary = catalogueItem.Solution.Summary;
            Description = catalogueItem.Solution.FullDescription;
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
    }
}
