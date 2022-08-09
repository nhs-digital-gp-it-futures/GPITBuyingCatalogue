﻿using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ClientApplicationType.BrowserBased
{
    public static class PlugInsOrExtensionsModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_PluginsRequiredNullOrEmpty_SetsModelError(
            string pluginsRequired,
            PlugInsOrExtensionsModel model,
            PlugInsOrExtensionsModelValidator validator)
        {
            model.PlugInsRequired = pluginsRequired;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PlugInsRequired)
                .WithErrorMessage(PlugInsOrExtensionsModelValidator.PluginsOrExtensionsRequiredError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            PlugInsOrExtensionsModel model,
            PlugInsOrExtensionsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
