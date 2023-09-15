using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
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
        ApplicableDays = competition.NonPriceElements?.ServiceLevel?.ApplicableDays;
        CanDelete = competition.NonPriceElements?.ServiceLevel is not null;
    }

    public string CompetitionName { get; set; }

    [ModelBinder(typeof(TimeInputModelBinder))]
    public DateTime? TimeFrom { get; set; }

    [ModelBinder(typeof(TimeInputModelBinder))]
    public DateTime? TimeUntil { get; set; }

    [StringLength(1000)]
    public string ApplicableDays { get; set; }
}
