using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public sealed class CommencementDateModelValidator : AbstractValidator<CommencementDateModel>
    {
        public const int MaximumInitialPeriod = 12;

        public const string CommencementDateInThePastErrorMessage = "Commencement date must be in the future";
        public const string InitialPeriodMissingErrorMessage = "Enter an initial period";
        public const string InitialPeriodNotANumberErrorMessage = "Initial period must be a number";
        public const string InitialPeriodTooLowErrorMessage = "Initial period must be greater than zero";
        public const string MaximumTermInvalidErrorMessage = "Maximum term must be at least a month longer than the initial period";
        public const string MaximumTermMissingErrorMessage = "Enter a maximum term";
        public const string MaximumTermNotANumberErrorMessage = "Maximum term must be a number";
        public const string MaximumTermTooLowErrorMessage = "Maximum term must be greater than zero";
        public const string MaximumTermTooHighErrorMessage = "Maximum term cannot be more than {0} months";

        public static readonly string InitialPeriodTooHighErrorMessage = $"Initial period cannot be more than {MaximumInitialPeriod} months";

        public CommencementDateModelValidator()
        {
            Include(new DateInputModelValidator());

            RuleFor(x => x.Date)
                .Must(x => x > DateTime.UtcNow.Date)
                .Unless(x => x.Date == null)
                .WithMessage(CommencementDateInThePastErrorMessage)
                .OverridePropertyName(x => x.Day);

            RuleFor(x => x.InitialPeriod)
                .IsNumericAndNonZero("initial period")
                .Must(InitialPeriodLessThanOrEqualToMaximum)
                .WithMessage(InitialPeriodTooHighErrorMessage);

            RuleFor(x => x.MaximumTerm)
                .IsNumericAndNonZero("maximum term")
                .Must(MaximumTermLessThanOrEqualToMaximum)
                .WithMessage(model => string.Format(MaximumTermTooHighErrorMessage, GetMaximumTerm(model)))
                .Must(MaximumTermGreaterThanInitialPeriod)
                .WithMessage(MaximumTermInvalidErrorMessage);
        }

        private static int GetMaximumTerm(CommencementDateModel model)
            => model.OrderTriageValue == OrderTriageValue.Under40K
                ? 12
                : 36;

        private static bool InitialPeriodLessThanOrEqualToMaximum(CommencementDateModel model, string initialPeriod)
            => model.InitialPeriodValue <= MaximumInitialPeriod;

        private static bool MaximumTermLessThanOrEqualToMaximum(CommencementDateModel model, string initialPeriod)
            => model.MaximumTermValue <= GetMaximumTerm(model);

        private static bool MaximumTermGreaterThanInitialPeriod(CommencementDateModel model, string initialPeriod)
            => model.MaximumTermValue > model.InitialPeriodValue;
    }
}
