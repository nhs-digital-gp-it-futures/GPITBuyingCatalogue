using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class EditServiceAvailabilityTimesModel : NavBaseModel
    {
        public EditServiceAvailabilityTimesModel()
        {
        }

        public EditServiceAvailabilityTimesModel(CatalogueItem solution)
            : this()
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
        }

        public EditServiceAvailabilityTimesModel(
            CatalogueItem solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes)
            : this(solution)
        {
            ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id;
            SupportType = serviceAvailabilityTimes.Category;
            From = serviceAvailabilityTimes.TimeFrom;
            Until = serviceAvailabilityTimes.TimeUntil;
            ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>()
                .Select(
                    x => new SelectOption<Iso8601DayOfWeek>(
                        x.ToString(),
                        x,
                        serviceAvailabilityTimes.IncludedDays?.Contains(x) ?? false))
                .ToList();
            IncludesBankHolidays = serviceAvailabilityTimes.IncludesBankHolidays;
            AdditionalInformation = serviceAvailabilityTimes.AdditionalInformation;
        }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public int? ServiceAvailabilityTimesId { get; init; }

        [StringLength(100)]
        public string SupportType { get; init; }

        [ModelBinder(BinderType = typeof(TimeInputModelBinder))]
        public DateTime? From { get; init; }

        [ModelBinder(BinderType = typeof(TimeInputModelBinder))]
        public DateTime? Until { get; init; }

        public IList<SelectOption<Iso8601DayOfWeek>> ApplicableDays { get; init; } = Enum.GetValues<Iso8601DayOfWeek>()
            .Select(x => new SelectOption<Iso8601DayOfWeek>(x.ToString(), x))
            .ToList();

        public bool? IncludesBankHolidays { get; set; }

        [StringLength(500)]
        public string AdditionalInformation { get; set; }

        public IEnumerable<SelectOption<bool>> BankHolidayOptions => new List<SelectOption<bool>>
        {
            new("Yes, include Bank Holidays", true), new("No, do not include Bank Holidays", false),
        };

        public bool CanDelete { get; init; }
    }
}
