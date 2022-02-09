using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public sealed class CommencementDateModelValidator : AbstractValidator<CommencementDateModel>
    {
        public const int MaximumInitialPeriod = 6;
        public const int MaximumMaximumTerm = 36;

        public const string CommencementDateDayMissingErrorMessage = "Approximate start date must include a day";
        public const string CommencementDateInThePastErrorMessage = "Approximate start date must be in the future";
        public const string CommencementDateInvalidErrorMessage = "Approximate start date must be a real date";
        public const string CommencementDateMonthMissingErrorMessage = "Approximate start date must include a month";
        public const string CommencementDateYearMissingErrorMessage = "Approximate start date must include a year";
        public const string CommencementDateYearTooShortErrorMessage = "Year must be four numbers";
        public const string InitialPeriodMissingErrorMessage = "Enter an initial period";
        public const string InitialPeriodNotANumberErrorMessage = "Initial period must be a number";
        public const string InitialPeriodTooLowErrorMessage = "Initial period must be greater than zero";
        public const string MaximumTermInvalidErrorMessage = "Maximum term must be at least a month longer than the initial period";
        public const string MaximumTermMissingErrorMessage = "Enter a maximum term";
        public const string MaximumTermNotANumberErrorMessage = "Maximum term must be a number";
        public const string MaximumTermTooLowErrorMessage = "Maximum term must be greater than zero";

        public static readonly string InitialPeriodTooHighErrorMessage = $"Initial period cannot be more than {MaximumInitialPeriod} months";
        public static readonly string MaximumTermTooHighErrorMessage = $"Maximum term cannot be more than {MaximumMaximumTerm} months";

        public CommencementDateModelValidator()
        {
            RuleFor(x => x.Day)
                .NotEmpty()
                .WithMessage(CommencementDateDayMissingErrorMessage);

            RuleFor(x => x.Month)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Day))
                .WithMessage(CommencementDateMonthMissingErrorMessage)
                .OverridePropertyName(x => x.Day);

            RuleFor(x => x.Year)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Month))
                .WithMessage(CommencementDateYearMissingErrorMessage)
                .Length(4)
                .WithMessage(CommencementDateYearTooShortErrorMessage)
                .OverridePropertyName(x => x.Day);

            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .Must(x => x.CommencementDate != null)
                .Unless(CommencementDateIsInvalid)
                .WithMessage(CommencementDateInvalidErrorMessage)
                .Must(x => x.CommencementDate > DateTime.UtcNow.Date)
                .Unless(CommencementDateIsInvalid)
                .WithMessage(CommencementDateInThePastErrorMessage)
                .OverridePropertyName(wp => wp.Day);

            RuleFor(x => x.InitialPeriod)
                .Cascade(CascadeMode.Stop)
                .IsNumericAndNonZero("initial period")
                .Must(InitialPeriodLessThanOrEqualToMaximum)
                .WithMessage(InitialPeriodTooHighErrorMessage);

            RuleFor(x => x.MaximumTerm)
                .Cascade(CascadeMode.Stop)
                .IsNumericAndNonZero("maximum term")
                .Must(MaximumTermLessThanOrEqualToMaximum)
                .WithMessage(MaximumTermTooHighErrorMessage)
                .Must(MaximumTermGreaterThanInitialPeriod)
                .WithMessage(MaximumTermInvalidErrorMessage);
        }

        private static bool CommencementDateIsInvalid(CommencementDateModel model)
        {
            return string.IsNullOrWhiteSpace(model.Day)
                || string.IsNullOrWhiteSpace(model.Month)
                || string.IsNullOrWhiteSpace(model.Year)
                || model.Year.Length != 4;
        }

        private static bool InitialPeriodLessThanOrEqualToMaximum(CommencementDateModel model, string initialPeriod)
            => model.InitialPeriodValue <= MaximumInitialPeriod;

        private static bool MaximumTermLessThanOrEqualToMaximum(CommencementDateModel model, string initialPeriod)
            => model.MaximumTermValue <= MaximumMaximumTerm;

        private static bool MaximumTermGreaterThanInitialPeriod(CommencementDateModel model, string initialPeriod)
            => model.MaximumTermValue > model.InitialPeriodValue;
    }
}
