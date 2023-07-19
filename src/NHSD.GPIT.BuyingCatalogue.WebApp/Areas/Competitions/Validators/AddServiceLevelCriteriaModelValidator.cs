using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class AddServiceLevelCriteriaModelValidator : AbstractValidator<AddServiceLevelCriteriaModel>
{
    internal const string EmptyApplicableDaysError = "Enter the applicable days";
    internal const string EmptyTimeFromError = "Enter a from time";
    internal const string EmptyTimeUntilError = "Enter an until time";

    public AddServiceLevelCriteriaModelValidator()
    {
        RuleFor(x => x.ApplicableDays)
            .NotEmpty()
            .WithMessage(EmptyApplicableDaysError);

        RuleFor(x => x.TimeFrom)
            .NotNull()
            .WithMessage(EmptyTimeFromError);

        RuleFor(x => x.TimeUntil)
            .NotNull()
            .WithMessage(EmptyTimeUntilError)
            .Unless(x => !x.TimeFrom.HasValue);
    }
}
