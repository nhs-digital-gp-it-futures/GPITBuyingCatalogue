﻿using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;

public sealed class AddEditDataProcessingInformationModelValidator : AbstractValidator<AddEditDataProcessingInformationModel>
{
    public AddEditDataProcessingInformationModelValidator()
    {
        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage(SharedErrorMessages.SubjectMatterError);

        RuleFor(x => x.Duration)
            .NotEmpty()
            .WithMessage(SharedErrorMessages.DurationError);

        RuleFor(x => x.ProcessingNature)
            .NotEmpty()
            .WithMessage(SharedErrorMessages.ProcessingNatureError);

        RuleFor(x => x.PersonalDataTypes)
            .NotEmpty()
            .WithMessage(SharedErrorMessages.PersonalDataTypesError);

        RuleFor(x => x.DataSubjectCategories)
            .NotEmpty()
            .WithMessage(SharedErrorMessages.DataSubjectCategoriesError);

        RuleFor(x => x.ProcessingLocation)
            .NotEmpty()
            .WithMessage(SharedErrorMessages.ProcessingLocationError);
    }
}
