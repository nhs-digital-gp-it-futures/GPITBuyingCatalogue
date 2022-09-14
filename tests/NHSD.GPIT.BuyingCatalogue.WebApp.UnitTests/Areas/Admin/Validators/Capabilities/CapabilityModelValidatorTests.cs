﻿using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Capabilities;

public static class CapabilityModelValidatorTests
{
    [Theory]
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
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
