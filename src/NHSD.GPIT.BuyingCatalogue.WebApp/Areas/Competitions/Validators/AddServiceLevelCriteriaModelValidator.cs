using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class AddServiceLevelCriteriaModelValidator : AbstractValidator<AddServiceLevelCriteriaModel>
{
    internal const string EmptyApplicableDaysError = "Select applicable days";
    internal const string EmptyTimeFromError = "Enter a from time";
    internal const string EmptyTimeUntilError = "Enter an until time";
    internal const string MissingBankHolidaysError = "Select yes if you want to include Bank Holidays";

    public AddServiceLevelCriteriaModelValidator()
    {
        RuleFor(x => x.ApplicableDays)
            .Must(x => x.Any(y => y.Selected))
            .WithMessage(EmptyApplicableDaysError)
            .OverridePropertyName($"{nameof(AddServiceLevelCriteriaModel.ApplicableDays)}[0].Selected");

        RuleFor(x => x.TimeFrom)
            .NotNull()
            .WithMessage(EmptyTimeFromError);

        RuleFor(x => x.TimeUntil)
            .NotNull()
            .WithMessage(EmptyTimeUntilError)
            .Unless(x => !x.TimeFrom.HasValue);

        RuleFor(x => x.IncludesBankHolidays)
            .NotNull()
            .WithMessage(MissingBankHolidaysError);
    }
}
