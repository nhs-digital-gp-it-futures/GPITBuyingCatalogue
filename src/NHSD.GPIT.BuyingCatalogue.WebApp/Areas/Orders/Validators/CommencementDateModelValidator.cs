using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public sealed class CommencementDateModelValidator : AbstractValidator<CommencementDateModel>
    {
        public const string CommencementDateInThePastErrorMessage = "Commencement date must be no more than 3 months in the past";
        public const string InitialPeriodMissingErrorMessage = "Enter an initial period";
        public const string InitialPeriodNotANumberErrorMessage = "Initial period must be a number";
        public const string InitialPeriodTooLowErrorMessage = "Initial period must be greater than zero";
        public const string DurationInvalidErrorMessage = "Duration must be at least a month longer than the initial period";
        public const string MaximumTermMissingErrorMessage = "Enter a maximum term";
        public const string MaximumTermNotANumberErrorMessage = "Maximum term must be a number";
        public const string MaximumTermTooLowErrorMessage = "Maximum term must be greater than zero";

        public static readonly string MaximumTermTooHighErrorMessage = "Duration cannot be more than {0} months";

        public CommencementDateModelValidator()
        {
            Include(new DateInputModelValidator());

            RuleFor(x => x.Date)
                .Must(x => x >= DateTime.UtcNow.Date.AddMonths(-3))
                .Unless(x => x.Date == null)
                .WithMessage(CommencementDateInThePastErrorMessage)
                .OverridePropertyName(x => x.Day);

            RuleFor(x => x.InitialPeriod)
                .IsNumericAndNonZero("initial period");

            RuleFor(x => x.MaximumTerm)
                .IsNumericAndNonZero("maximum term")
                .Must((model, _) => MaximumTermLessThanOrEqualToMaximum(model))
                .WithMessage(model => string.Format(MaximumTermTooHighErrorMessage, model.MaxumimTermUpperLimit))
                .Must((model, _) => MaximumTermGreaterThanInitialPeriod(model))
                .WithMessage(DurationInvalidErrorMessage);
        }

        private static bool MaximumTermLessThanOrEqualToMaximum(CommencementDateModel model)
            => model.MaximumTermValue <= model.MaxumimTermUpperLimit;

        private static bool MaximumTermGreaterThanInitialPeriod(CommencementDateModel model)
            => model.MaximumTermValue > model.InitialPeriodValue;
    }
}
