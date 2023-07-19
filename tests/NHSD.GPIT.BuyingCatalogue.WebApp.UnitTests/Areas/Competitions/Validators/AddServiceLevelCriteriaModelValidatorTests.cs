using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class AddServiceLevelCriteriaModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NullApplicableDays_SetsModelError(
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.ApplicableDays = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ApplicableDays)
            .WithErrorMessage(AddServiceLevelCriteriaModelValidator.EmptyApplicableDaysError);
    }

    [Theory]
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
    public static void Validate_Valid_NoModelError(
        string applicableDays,
        AddServiceLevelCriteriaModel model,
        AddServiceLevelCriteriaModelValidator validator)
    {
        model.ApplicableDays = applicableDays;
        model.TimeFrom = DateTime.UtcNow;
        model.TimeUntil = DateTime.UtcNow;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
