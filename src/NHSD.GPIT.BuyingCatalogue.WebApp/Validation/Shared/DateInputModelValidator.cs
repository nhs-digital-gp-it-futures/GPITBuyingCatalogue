using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared
{
    public class DateInputModelValidator : AbstractValidator<DateInputModel>
    {
        public const string DateInvalidErrorMessage = "Date must be a real date";
        public const string DayMissingErrorMessage = "Date must include a day";
        public const string MonthMissingErrorMessage = "Date must include a month";
        public const string YearMissingErrorMessage = "Date must include a year";
        public const string YearWrongLengthErrorMessage = "Year must be four numbers";

        public DateInputModelValidator()
        {
            RuleFor(x => x.Day)
                .NotEmpty()
                .WithMessage(DayMissingErrorMessage);

            RuleFor(x => x.Month)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Day))
                .WithMessage(MonthMissingErrorMessage);

            RuleFor(x => x.Year)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Month))
                .WithMessage(YearMissingErrorMessage)
                .Length(4)
                .WithMessage(YearWrongLengthErrorMessage);

            RuleFor(x => x)
                .Must(x => x.Date != null)
                .Unless(x => !x.IsComplete)
                .WithMessage(DateInvalidErrorMessage)
                .OverridePropertyName(wp => wp.Day);
        }
    }
}
