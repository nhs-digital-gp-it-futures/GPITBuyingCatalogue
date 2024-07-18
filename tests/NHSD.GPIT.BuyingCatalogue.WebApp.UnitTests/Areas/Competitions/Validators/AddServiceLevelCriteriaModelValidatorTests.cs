using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class AddServiceLevelCriteriaModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoSelectedApplicableDays_SetsModelError(
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>()
            .Select(x => new SelectOption<Iso8601DayOfWeek>(x.ToString(), x))
            .ToList();

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor($"ApplicableDays[0].Selected")
            .WithErrorMessage(AddServiceLevelCriteriaModelValidator.EmptyApplicableDaysError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NullTimeFrom_SetsModelError(
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.TimeFrom = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.TimeFrom)
            .WithErrorMessage(AddServiceLevelCriteriaModelValidator.EmptyTimeFromError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NullBankHolidaysSelection_SetsModelError(
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.IncludesBankHolidays = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IncludesBankHolidays)
            .WithErrorMessage(AddServiceLevelCriteriaModelValidator.MissingBankHolidaysError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NullTimeUntil_SetsModelError(
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.TimeFrom = DateTime.UtcNow;
        model.TimeUntil = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.TimeUntil)
            .WithErrorMessage(AddServiceLevelCriteriaModelValidator.EmptyTimeUntilError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelError(
        List<Iso8601DayOfWeek> applicableDays,
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.ApplicableDays = applicableDays.Select(x => new SelectOption<Iso8601DayOfWeek>(x.ToString(), x, true))
            .ToList();
        model.TimeFrom = DateTime.UtcNow;
        model.TimeUntil = DateTime.UtcNow;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
