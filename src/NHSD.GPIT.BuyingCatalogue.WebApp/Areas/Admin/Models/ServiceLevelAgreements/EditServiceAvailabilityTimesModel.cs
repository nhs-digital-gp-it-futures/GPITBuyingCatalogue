using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class EditServiceAvailabilityTimesModel : NavBaseModel
    {
        public EditServiceAvailabilityTimesModel()
        {
            BackLinkText = "Go back";
        }

        public EditServiceAvailabilityTimesModel(CatalogueItem solution)
            : this()
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
        }

        public EditServiceAvailabilityTimesModel(CatalogueItem solution, ServiceAvailabilityTimes serviceAvailabilityTimes)
            : this(solution)
        {
            ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id;
            SupportType = serviceAvailabilityTimes.Category;
            From = serviceAvailabilityTimes.TimeFrom;
            Until = serviceAvailabilityTimes.TimeUntil;
            ApplicableDays = serviceAvailabilityTimes.ApplicableDays;
        }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public int? ServiceAvailabilityTimesId { get; init; }

        public string SupportType { get; init; }

        public DateTime? From { get; init; }

        public DateTime? Until { get; init; }

        public string ApplicableDays { get; init; }

        public bool CanDelete { get; init; }
    }
}
