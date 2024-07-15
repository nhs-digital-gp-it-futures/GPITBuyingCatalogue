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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockAutoData]
        public static void Validate_InvalidCsvUrl_SetsModelError(
            ImportGpPracticeListModel model,
            [Frozen] IUrlValidator mockUrlValidator,
            ImportGpPracticeListModelValidator validator)
        {
            model.CsvUrl = Url;

            mockUrlValidator.IsValidUrl(Url).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.CsvUrl)
                .WithErrorMessage(FluentValidationExtensions.InvalidUrlErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidCsvUrl_NoErrors(
            ImportGpPracticeListModel model,
            [Frozen] IUrlValidator mockUrlValidator,
            ImportGpPracticeListModelValidator validator)
        {
            model.CsvUrl = Url;

            mockUrlValidator.IsValidUrl(Url).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
