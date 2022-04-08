﻿using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AssociatedServices
{
    public static class AddAssociatedServicesModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            string additionalServiceRequired,
            AddAssociatedServicesModel model,
            AddAssociatedServicesModelValidator systemUnderTest)
        {
            model.AdditionalServicesRequired = additionalServiceRequired;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.AdditionalServicesRequired)
                .WithErrorMessage(AddAssociatedServicesModelValidator.AdditionalServicesRequiredMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValuesProvided_NoValidationErrors(
            AddAssociatedServicesModel model,
            AddAssociatedServicesModelValidator systemUnderTest)
        {
            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
