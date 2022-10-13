using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared
{
    public class DateInputModelValidator : AbstractValidator<DateInputModel>
    {
        public const string DateInvalidErrorMessage = "Date must be a real date";
        public const string DateInvalidWithDescriptionErrorMessage = "Date for {0} must be a real date";
        public const string DayMissingErrorMessage = "Date must include a day";
        public const string DayMissingWithDescriptionErrorMessage = "Date for {0} must include a day";
        public const string MonthMissingErrorMessage = "Date must include a month";
        public const string MonthMissingWithDescriptionErrorMessage = "Date for {0} must include a month";
        public const string YearMissingErrorMessage = "Date must include a year";
        public const string YearMissingWithDescriptionErrorMessage = "Date for {0} must include a year";
        public const string YearWrongLengthErrorMessage = "Year must be four numbers";
        public const string YearWrongLengthWithDescriptionErrorMessage = "Year for {0} must be four numbers";

        public DateInputModelValidator()
        {
            RuleFor(x => x.Day)
                .NotEmpty()
                .WithMessage(x => DayMissing(x.Description));

            RuleFor(x => x.Month)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Day))
                .WithMessage(x => MonthMissing(x.Description));

            RuleFor(x => x.Year)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Month))
                .WithMessage(x => YearMissing(x.Description))
                .Length(4)
                .WithMessage(x => YearWrongLength(x.Description));

            RuleFor(x => x)
                .Must(x => x.Date != null)
                .Unless(x => !x.IsComplete)
                .WithMessage(x => DateInvalid(x.Description))
                .OverridePropertyName(x => x.Day);
        }

        private static string DayMissing(string description) => string.IsNullOrWhiteSpace(description)
            ? DayMissingErrorMessage
            : string.Format(DayMissingWithDescriptionErrorMessage, description);

        private static string MonthMissing(string description) => string.IsNullOrWhiteSpace(description)
            ? MonthMissingErrorMessage
            : string.Format(MonthMissingWithDescriptionErrorMessage, description);

        private static string YearMissing(string description) => string.IsNullOrWhiteSpace(description)
            ? YearMissingErrorMessage
            : string.Format(YearMissingWithDescriptionErrorMessage, description);

        private static string YearWrongLength(string description) => string.IsNullOrWhiteSpace(description)
            ? YearWrongLengthErrorMessage
            : string.Format(YearWrongLengthWithDescriptionErrorMessage, description);

        private static string DateInvalid(string description) => string.IsNullOrWhiteSpace(description)
            ? DateInvalidErrorMessage
            : string.Format(DateInvalidWithDescriptionErrorMessage, description);
    }
}
