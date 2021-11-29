﻿using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class OperatingSystemsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectedOperatingSystems_SetsModelError(
            OperatingSystemsModel model,
            OperatingSystemsModelValidator validator)
        {
            model.OperatingSystems.ToList().ForEach(os => os.Checked = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("OperatingSystems[0].Checked")
                .WithErrorMessage("Select at least one supported operating system");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedOperatingSystem_NoModelError(
            OperatingSystemsModel model,
            OperatingSystemsModelValidator validator)
        {
            model.OperatingSystems[0].Checked = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
