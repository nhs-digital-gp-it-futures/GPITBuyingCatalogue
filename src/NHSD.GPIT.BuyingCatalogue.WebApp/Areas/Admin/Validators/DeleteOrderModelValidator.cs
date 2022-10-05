using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class DeleteOrderModelValidator : AbstractValidator<DeleteOrderModel>
    {
        public const string ApprovalDateDayMissingErrorMessage = "Approval date must include a day";
        public const string ApprovalDateInTheFutureErrorMessage = "Approval date must not be in the future";
        public const string ApprovalDateInvalidErrorMessage = "Approval date must be a real date";
        public const string ApprovalDateMonthMissingErrorMessage = "Approval date must include a month";
        public const string ApprovalDateYearMissingErrorMessage = "Approval date must include a year";
        public const string ApprovalDateYearTooShortErrorMessage = "Year must be four numbers";
        public const string NameOfApproverMissingErrorMessage = "Name of the person approving the delete is required";
        public const string NameOfRequesterMissingErrorMessage = "Name of the person requesting the delete is required";
        public const string ApprovalDateBeforeOrderCreationErrorMessage = "Date must be on or after the date the order was created ({0})";

        public DeleteOrderModelValidator()
        {
            RuleFor(x => x.ApprovalDay)
                .NotEmpty()
                .WithMessage(ApprovalDateDayMissingErrorMessage);

            RuleFor(x => x.ApprovalMonth)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.ApprovalDay))
                .WithMessage(ApprovalDateMonthMissingErrorMessage);

            RuleFor(x => x.ApprovalYear)
                .NotEmpty()
                .Unless(x => string.IsNullOrWhiteSpace(x.ApprovalMonth))
                .WithMessage(ApprovalDateYearMissingErrorMessage)
                .Length(4)
                .WithMessage(ApprovalDateYearTooShortErrorMessage);

            RuleFor(x => x)
                .Must(x => x.ApprovalDate != null)
                .Unless(ApprovalDateIsInvalid)
                .WithMessage(ApprovalDateInvalidErrorMessage)
                .Must(x => x.ApprovalDate <= DateTime.UtcNow.Date)
                .Unless(ApprovalDateIsInvalid)
                .WithMessage(ApprovalDateInTheFutureErrorMessage)
                .Must(x => x.ApprovalDate >= x.OrderCreationDate.Date)
                .Unless(ApprovalDateIsInvalid)
                .WithMessage(x => string.Format(ApprovalDateBeforeOrderCreationErrorMessage, x.OrderCreationDate.ToLongDateString()))
                .OverridePropertyName(x => x.ApprovalDay);

            RuleFor(x => x.NameOfApprover)
                .NotEmpty()
                .WithMessage(NameOfApproverMissingErrorMessage);

            RuleFor(x => x.NameOfRequester)
                .NotEmpty()
                .WithMessage(NameOfRequesterMissingErrorMessage);
        }

        private static bool ApprovalDateIsInvalid(DeleteOrderModel model)
        {
            return string.IsNullOrWhiteSpace(model.ApprovalDay)
                || string.IsNullOrWhiteSpace(model.ApprovalMonth)
                || string.IsNullOrWhiteSpace(model.ApprovalYear)
                || model.ApprovalYear.Length != 4;
        }
    }
}
