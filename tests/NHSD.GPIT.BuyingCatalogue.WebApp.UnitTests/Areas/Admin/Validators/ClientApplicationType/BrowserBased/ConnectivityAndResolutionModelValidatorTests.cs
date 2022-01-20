﻿using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ClientApplicationType.BrowserBased
{
    public static class ConnectivityAndResolutionModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_SelectedConnectionSpeedNullOrEmpty_SetsModelError(
            string selectedConnectionSpeed,
            ConnectivityAndResolutionModel model,
            ConnectivityAndResolutionModelValidator validator)
        {
            model.SelectedConnectionSpeed = selectedConnectionSpeed;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedConnectionSpeed)
                .WithErrorMessage("Select a connection speed");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedConnectionSpeedValid_NoModelError(
            string selectedConnectionSpeed,
            ConnectivityAndResolutionModel model,
            ConnectivityAndResolutionModelValidator validator)
        {
            model.SelectedConnectionSpeed = selectedConnectionSpeed;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
