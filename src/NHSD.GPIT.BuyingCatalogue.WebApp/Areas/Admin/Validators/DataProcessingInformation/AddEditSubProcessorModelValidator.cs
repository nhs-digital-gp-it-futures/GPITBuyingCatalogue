using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;

public sealed class AddEditSubProcessorModelValidator : AbstractValidator<AddEditSubProcessorModel>
{
    internal const string OrganisationNameError = "Enter a sub-processor organisation name";

    internal const string PostProcessingPlanError =
        "Enter plan for return and destruction of data once processing is complete";

    public AddEditSubProcessorModelValidator()
    {
        RuleFor(x => x.OrganisationName)
            .NotEmpty()
            .WithMessage(OrganisationNameError);

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

        RuleFor(x => x.PostProcessingPlan)
            .NotEmpty()
            .WithMessage(PostProcessingPlanError);
    }
}
