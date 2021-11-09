using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class AddEditServiceLevelModel : NavBaseModel
    {
        public AddEditServiceLevelModel()
        {
            BackLinkText = "Go back";
        }

        public AddEditServiceLevelModel(CatalogueItem solution)
            : this()
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
        }

        public AddEditServiceLevelModel(CatalogueItem solution, SlaServiceLevel serviceLevel)
            : this(solution)
        {
            ServiceLevelId = serviceLevel.Id;
            ServiceType = serviceLevel.TypeOfService;
            ServiceLevel = serviceLevel.ServiceLevel;
            HowMeasured = serviceLevel.HowMeasured;
            CreditsApplied = serviceLevel.ServiceCredits;
        }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public int? ServiceLevelId { get; init; }

        public string ServiceType { get; init; }

        [StringLength(1000)]
        public string ServiceLevel { get; init; }

        [StringLength(1000)]
        public string HowMeasured { get; init; }

        public bool? CreditsApplied { get; init; }

        public IList<SelectListItem> CreditsOptions => new List<SelectListItem>
        {
            new(true.ToYesNo(), true.ToString()),
            new(false.ToYesNo(), false.ToString()),
        };

        public bool CanDelete { get; init; }
    }
}
