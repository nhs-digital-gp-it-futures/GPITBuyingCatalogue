﻿using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.DesktopBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ClientApplicationType.DesktopBased
{
    public static class ConnectivityModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_SelectedConnectionSpeedNullOrEmpty_SetsModelError(
            string selectedConnectionSpeed,
            ConnectivityModel model,
            ConnectivityModelValidator validator)
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
            ConnectivityModel model,
            ConnectivityModelValidator validator)
        {
            model.SelectedConnectionSpeed = selectedConnectionSpeed;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
