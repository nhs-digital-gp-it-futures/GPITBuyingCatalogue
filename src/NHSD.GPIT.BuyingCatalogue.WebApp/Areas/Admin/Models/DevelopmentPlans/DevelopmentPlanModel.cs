using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans
{
    public sealed class DevelopmentPlanModel : NavBaseModel
    {
        public DevelopmentPlanModel()
        {
        }

        public DevelopmentPlanModel(CatalogueItem catalogueItem)
        {
            catalogueItem.ValidateNotNull(nameof(catalogueItem));

            Link = catalogueItem.Solution?.RoadMap;
            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;
            WorkOffPlans = catalogueItem.Solution.WorkOffPlans;
        }

        [StringLength(1000)]
        public string Link { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public ICollection<WorkOffPlan> WorkOffPlans { get; init; }
    }
}
