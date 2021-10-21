using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class FeaturesModel
    {
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

        public FeaturesModel FromCatalogueItem(CatalogueItem catalogueItem)
        {
            catalogueItem.ValidateNotNull(nameof(catalogueItem));

            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;

            var featuresToSet = catalogueItem.Features() ?? Array.Empty<string>();
            for (var i = 0; i < featuresToSet.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Feature01 = featuresToSet[i];
                        break;
                    case 1:
                        Feature02 = featuresToSet[i];
                        break;
                    case 2:
                        Feature03 = featuresToSet[i];
                        break;
                    case 3:
                        Feature04 = featuresToSet[i];
                        break;
                    case 4:
                        Feature05 = featuresToSet[i];
                        break;
                    case 5:
                        Feature06 = featuresToSet[i];
                        break;
                    case 6:
                        Feature07 = featuresToSet[i];
                        break;
                    case 7:
                        Feature08 = featuresToSet[i];
                        break;
                    case 8:
                        Feature09 = featuresToSet[i];
                        break;
                    case 9:
                        Feature10 = featuresToSet[i];
                        break;
                }
            }

            return this;
        }

        public TaskProgress Status() =>
            AllFeatures.Any()
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
    }
}
