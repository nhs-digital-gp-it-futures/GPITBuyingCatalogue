using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.DeleteOrder
{
    public class TerminateOrderModelValidator : AbstractValidator<TerminateOrderModel>
    {
        public const string ReasonEmptyErrorMessage = "Provide a reason why the contract is being terminated";
        public const string TerminationDateDayMissingErrorMessage = "Termination date must include a day";
        public const string TerminationDateInThePastErrorMessage = "Termination date must be in the future";
        public const string TerminationDateInvalidErrorMessage = "Termination date must be a real date";
        public const string TerminationDateMonthMissingErrorMessage = "Termination date must include a month";
        public const string TerminationDateYearMissingErrorMessage = "Termination date must include a year";
        public const string TerminationDateYearTooShortErrorMessage = "Year must be four numbers";
        public const string ConfirmErrorMessage = "Confirm you want to terminate this contract";

        public TerminateOrderModelValidator()
        {
            RuleFor(m => m.Reason)
                .NotNull()
                .WithMessage(ReasonEmptyErrorMessage);

            RuleFor(x => x.Day)
                .NotEmpty()
                .WithMessage(TerminationDateDayMissingErrorMessage);

            RuleFor(x => x.Month)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Day))
                .WithMessage(TerminationDateMonthMissingErrorMessage);

            RuleFor(x => x.Year)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.Month))
                .WithMessage(TerminationDateYearMissingErrorMessage)
                .Length(4)
                .WithMessage(TerminationDateYearTooShortErrorMessage);

            RuleFor(x => x)
                .Must(x => x.TerminationDate != null)
                .Unless(TerminationDateIsInvalid)
                .WithMessage(TerminationDateInvalidErrorMessage)
                .Must(x => x.TerminationDate.GetValueOrDefault().Date > DateTime.UtcNow.Date)
                .Unless(TerminationDateIsInvalid)
                .WithMessage(TerminationDateInThePastErrorMessage)
                .OverridePropertyName(x => x.Day);

            RuleFor(x => x.Confirm)
                .Equal(true)
                .WithMessage(ConfirmErrorMessage);
        }

        private static bool TerminationDateIsInvalid(TerminateOrderModel model)
        {
            return string.IsNullOrWhiteSpace(model.Day)
                || string.IsNullOrWhiteSpace(model.Month)
                || string.IsNullOrWhiteSpace(model.Year)
                || model.Year.Length != 4;
        }
    }
}
