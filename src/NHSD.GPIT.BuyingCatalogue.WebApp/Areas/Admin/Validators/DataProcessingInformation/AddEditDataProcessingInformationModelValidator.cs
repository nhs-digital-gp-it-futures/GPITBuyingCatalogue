using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;

public sealed class AddEditDataProcessingInformationModelValidator : AbstractValidator<AddEditDataProcessingInformationModel>
{
    public AddEditDataProcessingInformationModelValidator()
    {
        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage(DataProcessingValidationErrors.SubjectMatterError);

        RuleFor(x => x.Duration)
            .NotEmpty()
            .WithMessage(DataProcessingValidationErrors.DurationError);

        RuleFor(x => x.ProcessingNature)
            .NotEmpty()
            .WithMessage(DataProcessingValidationErrors.ProcessingNatureError);

        RuleFor(x => x.PersonalDataTypes)
            .NotEmpty()
            .WithMessage(DataProcessingValidationErrors.PersonalDataTypesError);

        RuleFor(x => x.DataSubjectCategories)
            .NotEmpty()
            .WithMessage(DataProcessingValidationErrors.DataSubjectCategoriesError);

        RuleFor(x => x.ProcessingLocation)
            .NotEmpty()
            .WithMessage(DataProcessingValidationErrors.ProcessingLocationError);
    }
}
