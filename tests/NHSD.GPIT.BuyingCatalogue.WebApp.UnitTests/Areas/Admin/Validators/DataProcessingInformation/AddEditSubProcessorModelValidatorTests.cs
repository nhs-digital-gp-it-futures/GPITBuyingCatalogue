using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.DataProcessingInformation;

public static class AddEditSubProcessorModelValidatorTests
{
    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingOrganisationName_SetsModelError(
        string organisationName,
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
    {
        model.OrganisationName = organisationName;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.OrganisationName)
            .WithErrorMessage(AddEditSubProcessorModelValidator.OrganisationNameError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingSubject_SetsModelError(
        string subject,
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
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
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
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
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
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
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
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
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
    {
        model.DataSubjectCategories = dataSubjectCategories;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DataSubjectCategories)
            .WithErrorMessage(DataProcessingValidationErrors.DataSubjectCategoriesError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_MissingPostProcessingPlan_SetsModelError(
        string postProcessingPlan,
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
    {
        model.PostProcessingPlan = postProcessingPlan;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PostProcessingPlan)
            .WithErrorMessage(AddEditSubProcessorModelValidator.PostProcessingPlanError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        AddEditSubProcessorModel model,
        AddEditSubProcessorModelValidator validator)
    {
        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
