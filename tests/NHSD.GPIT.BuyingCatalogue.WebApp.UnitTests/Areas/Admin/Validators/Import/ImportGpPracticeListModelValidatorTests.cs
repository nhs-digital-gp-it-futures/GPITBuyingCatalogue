using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Import;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Import
{
    public static class ImportGpPracticeListModelValidatorTests
    {
        private const string Url = "https://www.test.com";

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_NoCsvUrl_SetsModelError(
            string csvUrl,
            ImportGpPracticeListModel model,
            ImportGpPracticeListModelValidator validator)
        {
            model.CsvUrl = csvUrl;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.CsvUrl)
                .WithErrorMessage(ImportGpPracticeListModelValidator.CSvUrlErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidCsvUrl_SetsModelError(
            ImportGpPracticeListModel model,
            [Frozen] Mock<IUrlValidator> mockUrlValidator,
            ImportGpPracticeListModelValidator validator)
        {
            model.CsvUrl = Url;

            mockUrlValidator
                .Setup(x => x.IsValidUrl(Url))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            mockUrlValidator.VerifyAll();

            result.ShouldHaveValidationErrorFor(m => m.CsvUrl)
                .WithErrorMessage(FluentValidationExtensions.InvalidUrlErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidCsvUrl_NoErrors(
            ImportGpPracticeListModel model,
            [Frozen] Mock<IUrlValidator> mockUrlValidator,
            ImportGpPracticeListModelValidator validator)
        {
            model.CsvUrl = Url;

            mockUrlValidator
                .Setup(x => x.IsValidUrl(Url))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            mockUrlValidator.VerifyAll();

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
