﻿using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Hosting
{
    public sealed class OnPremiseModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_HostingModelNullOrEmpty_SetsModelError(
            string hostingModel,
            OnPremiseModel model,
            OnPremiseModelValidator validator)
        {
            model.HostingModel = hostingModel;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HostingModel);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NoLink_DoesNotValidate(
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            var model = new OnPremiseModel();

            var result = validator.TestValidate(model);

            urlValidator.Verify(uv => uv.IsValidUrl(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            OnPremiseModel model,
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a prefix to the URL, either http or https");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidLink_SetsModelError(
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            var model = new OnPremiseModel { Link = "http://wiothaoih" };

            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a valid URL");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidLink_NoModelError(
            Uri uri,
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            var model = new OnPremiseModel { Link = uri.ToString() };
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
