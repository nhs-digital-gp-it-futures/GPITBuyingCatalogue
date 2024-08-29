using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;

public sealed class AddEditDataProcessingInformationModelValidator : AbstractValidator<AddEditDataProcessingInformationModel>
{
    internal const string SubjectMatterError = "Enter information about the subject matter of the processing";
    internal const string DurationError = "Enter information about the duration of the processing";
    internal const string ProcessingNatureError = "Enter information about the nature and purposes of processing";
    internal const string PersonalDataTypesError = "Enter information about the types of personal data";
    internal const string DataSubjectCategoriesError = "Enter information about the categories of data subjects";
    internal const string ProcessingLocationError = "Enter information about the location of data processing ";

    public AddEditDataProcessingInformationModelValidator()
    {
        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage(SubjectMatterError);

        RuleFor(x => x.Duration)
            .NotEmpty()
            .WithMessage(DurationError);

        RuleFor(x => x.ProcessingNature)
            .NotEmpty()
            .WithMessage(ProcessingNatureError);

        RuleFor(x => x.PersonalDataTypes)
            .NotEmpty()
            .WithMessage(PersonalDataTypesError);

        RuleFor(x => x.DataSubjectCategories)
            .NotEmpty()
            .WithMessage(DataSubjectCategoriesError);

        RuleFor(x => x.ProcessingLocation)
            .NotEmpty()
            .WithMessage(ProcessingLocationError);
    }
}
