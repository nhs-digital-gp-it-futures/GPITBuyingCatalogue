﻿using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Shared
{
    public static class SelectServicesModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NotApplicable_NoValidationErrors(
            SelectServicesModel model,
            SelectServicesModelValidator systemUnderTest)
        {
            model.AssociatedServicesOnly = false;
            model.Services.ForEach(x => x.IsSelected = false);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectServicesModel model,
            SelectServicesModelValidator systemUnderTest)
        {
            model.AssociatedServicesOnly = true;
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectServicesModel model,
            SelectServicesModelValidator systemUnderTest)
        {
            model.AssociatedServicesOnly = true;
            model.Services.ForEach(x => x.IsSelected = false);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Services[0].IsSelected")
                .WithErrorMessage(SelectServicesModelValidator.NoSelectionMadeErrorMessage);
        }
    }
}
