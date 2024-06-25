using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Capabilities;

public static class CapabilityModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoSelectedMustEpics_SetsModelError(
        List<CapabilityEpicModel> mustEpics,
        CapabilityModel model,
        CapabilityModelValidator validator)
    {
        mustEpics.ForEach(e => e.Selected = false);
        model.MustEpics = mustEpics;
        model.Selected = true;

        var result = validator.TestValidate(model);

        result
            .ShouldHaveAnyValidationError()
            .WithErrorMessage(CapabilityModelValidator.SelectMustEpicError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NoMustEpics_NoModelError(
        CapabilityModel model,
        CapabilityModelValidator validator)
    {
        model.MustEpics = Enumerable.Empty<CapabilityEpicModel>().ToList();
        model.Selected = true;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MockAutoData]
    public static void Validate_CapabilityNotSelected_WithMustEpics_NoModelError(
        List<CapabilityEpicModel> mustEpics,
        CapabilityModel model,
        CapabilityModelValidator validator)
    {
        mustEpics.ForEach(e => e.Selected = false);
        model.MustEpics = mustEpics;
        model.Selected = false;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MockAutoData]
    public static void Validate_CapabilityNotSelected_NoMustEpics_NoModelError(
        CapabilityModel model,
        CapabilityModelValidator validator)
    {
        model.MustEpics = Enumerable.Empty<CapabilityEpicModel>().ToList();
        model.Selected = false;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MockAutoData]
    public static void Validate_CapabilitySelectedWithMustEpics_NoModelError(
        List<CapabilityEpicModel> mustEpics,
        CapabilityModel model,
        CapabilityModelValidator validator)
    {
        mustEpics.ForEach(e => e.Selected = true);
        model.MustEpics = mustEpics;
        model.Selected = true;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
