using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.DataProcessingInformation;

public static class AddEditDataProcessingInformationModelValidatorTests
{
    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingSubject_SetsModelError(
        string subject,
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        model.Subject = subject;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Subject)
            .WithErrorMessage(DataProcessingValidationErrors.SubjectMatterError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingDuration_SetsModelError(
        string duration,
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        model.Duration = duration;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Duration)
            .WithErrorMessage(DataProcessingValidationErrors.DurationError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingProcessingNature_SetsModelError(
        string processingNature,
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        model.ProcessingNature = processingNature;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProcessingNature)
            .WithErrorMessage(DataProcessingValidationErrors.ProcessingNatureError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingPersonalDataTypes_SetsModelError(
        string personalDataTypes,
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        model.PersonalDataTypes = personalDataTypes;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PersonalDataTypes)
            .WithErrorMessage(DataProcessingValidationErrors.PersonalDataTypesError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingDataSubjectCategories_SetsModelError(
        string dataSubjectCategories,
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        model.DataSubjectCategories = dataSubjectCategories;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DataSubjectCategories)
            .WithErrorMessage(DataProcessingValidationErrors.DataSubjectCategoriesError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingProcessingLocation_SetsModelError(
        string processingLocation,
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        model.ProcessingLocation = processingLocation;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProcessingLocation)
            .WithErrorMessage(DataProcessingValidationErrors.ProcessingLocationError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        AddEditDataProcessingInformationModel model,
        AddEditDataProcessingInformationModelValidator validator)
    {
        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
