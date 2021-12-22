using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class FeaturesModel : NavBaseModel
    {
        public FeaturesModel()
        {
        }

        public FeaturesModel(CatalogueItem catalogueItem)
        {
            catalogueItem.ValidateNotNull(nameof(catalogueItem));

            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;

            var featuresToSet = catalogueItem.Features();

            if (featuresToSet is not null)
            {
                Feature01 = featuresToSet.ElementAtOrDefault(0);
                Feature02 = featuresToSet.ElementAtOrDefault(1);
                Feature03 = featuresToSet.ElementAtOrDefault(2);
                Feature04 = featuresToSet.ElementAtOrDefault(3);
                Feature05 = featuresToSet.ElementAtOrDefault(4);
                Feature06 = featuresToSet.ElementAtOrDefault(5);
                Feature07 = featuresToSet.ElementAtOrDefault(6);
                Feature08 = featuresToSet.ElementAtOrDefault(7);
                Feature09 = featuresToSet.ElementAtOrDefault(8);
                Feature10 = featuresToSet.ElementAtOrDefault(9);
            }
        }

        [StringLength(100)]
        public string Feature01 { get; set; }

        [StringLength(100)]
        public string Feature02 { get; set; }

        [StringLength(100)]
        public string Feature03 { get; set; }

        [StringLength(100)]
        public string Feature04 { get; set; }

        [StringLength(100)]
        public string Feature05 { get; set; }

        [StringLength(100)]
        public string Feature06 { get; set; }

        [StringLength(100)]
        public string Feature07 { get; set; }

        [StringLength(100)]
        public string Feature08 { get; set; }

        [StringLength(100)]
        public string Feature09 { get; set; }

        [StringLength(100)]
        public string Feature10 { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public virtual string[] AllFeatures =>
            new[]
                {
                    Feature01,
                    Feature02,
                    Feature03,
                    Feature04,
                    Feature05,
                    Feature06,
                    Feature07,
                    Feature08,
                    Feature09,
                    Feature10,
                }.Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();
    }
}
