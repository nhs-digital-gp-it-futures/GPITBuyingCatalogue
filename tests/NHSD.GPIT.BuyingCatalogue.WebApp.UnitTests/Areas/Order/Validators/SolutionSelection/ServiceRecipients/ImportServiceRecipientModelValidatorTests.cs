using System.IO;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.ServiceRecipients;

public static class ImportServiceRecipientModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NoFileSpecified_SetsModelError(
        ImportServiceRecipientModel model,
        ImportServiceRecipientModelValidator validator)
    {
        model.File = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.File)
            .WithErrorMessage(FluentValidationExtensions.NoFileSpecified);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_IncorrectFileType_SetsModelError(
        ImportServiceRecipientModel model,
        ImportServiceRecipientModelValidator validator)
    {
        model.File = new FormFile(Stream.Null, 0, 0, "invalid.pdf", "invalid.pdf");

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.File)
            .WithErrorMessage(FluentValidationExtensions.InvalidFileType);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_FileSizeTooLarge_SetsModelError(
        ImportServiceRecipientModel model,
        ImportServiceRecipientModelValidator validator)
    {
        model.File = new FormFile(
            Stream.Null,
            0,
            ImportServiceRecipientModelValidator.Mb * (ImportServiceRecipientModelValidator.MaxMbSize + 1),
            "upload.csv",
            "upload.csv");

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.File)
            .WithErrorMessage(ImportServiceRecipientModelValidator.InvalidFileSize);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_ValidFile_NoModelErrors(
        ImportServiceRecipientModel model,
        ImportServiceRecipientModelValidator validator)
    {
        model.File = new FormFile(
            Stream.Null,
            0,
            ImportServiceRecipientModelValidator.Mb / (ImportServiceRecipientModelValidator.MaxMbSize + 1),
            "upload.csv",
            "upload.csv");

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
