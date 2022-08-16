﻿using FluentValidation.TestHelper;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Validators.Filters
{
    public static class FilterCapabilitiesModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            FilterCapabilitiesModel model,
            FilterCapabilitiesModelValidator validator)
        {
            model.SelectedItems.ForEach(x => x.Selected = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage(FilterCapabilitiesModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectionMade_NoErrors(
            FilterCapabilitiesModel model,
            FilterCapabilitiesModelValidator validator)
        {
            model.SelectedItems.ForEach(x => x.Selected = true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
