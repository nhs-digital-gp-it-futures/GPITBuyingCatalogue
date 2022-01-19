using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Hosting
{
    public static class BaseCloudModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_SummaryNullOrEmpty_SetsModelError(
            string summary,
            BaseCloudModel model,
            BaseCloudModelValidator validator)
        {
            model.Summary = summary;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Summary)
                .WithErrorMessage("Enter a summary");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            string summary,
            [Frozen] Mock<IUrlValidator> urlValidator,
            BaseCloudModel model,
            BaseCloudModelValidator validator)
        {
            model.Summary = summary;

            urlValidator.Setup(v => v.IsValidUrl(It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
