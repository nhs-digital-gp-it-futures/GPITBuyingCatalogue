using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class AddServiceLevelCriteriaModel : NonPriceElementBase
{
    public AddServiceLevelCriteriaModel()
    {
    }

    public AddServiceLevelCriteriaModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        TimeFrom = competition.NonPriceElements?.ServiceLevel?.TimeFrom;
        TimeUntil = competition.NonPriceElements?.ServiceLevel?.TimeUntil;
        CanDelete = competition.NonPriceElements?.ServiceLevel is not null;

        IncludesBankHolidays = competition.NonPriceElements?.ServiceLevel?.IncludesBankHolidays;
        ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>()
            .Select(
                x => new SelectOption<Iso8601DayOfWeek>(
                    x.ToString(),
                    x,
                    competition.NonPriceElements?.ServiceLevel?.ApplicableDays?.Contains(x) ?? false))
            .ToList();
    }

    public string CompetitionName { get; set; }

    [ModelBinder(typeof(TimeInputModelBinder))]
    public DateTime? TimeFrom { get; set; }

    [ModelBinder(typeof(TimeInputModelBinder))]
    public DateTime? TimeUntil { get; set; }

    public bool? IncludesBankHolidays { get; set; }

    public IEnumerable<SelectOption<bool>> BankHolidayOptions => new List<SelectOption<bool>>
    {
        new("Yes, include Bank Holidays", true), new("No, do not include Bank Holidays", false),
    };

    public IList<SelectOption<Iso8601DayOfWeek>> ApplicableDays { get; set; }
}
