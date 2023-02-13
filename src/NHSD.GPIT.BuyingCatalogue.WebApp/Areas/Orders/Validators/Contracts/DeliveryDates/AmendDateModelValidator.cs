using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    public class AmendDateModelValidator : AbstractValidator<AmendDateModel>
    {
        public const string DeliveryDateInThePastErrorMessage = "Date must be in the future";
        public const string DeliveryDateBeforeCommencementDateErrorMessage = "Date must be on or after your commencement date ({0})";
        public const string DeliveryDateExceedsMaximumTermErrorMessage = "Date cannot exceed the maximum term of the original contract ({0})";

        public AmendDateModelValidator()
        {
            Include(new DateInputModelValidator());

            RuleFor(x => x)
                .Must(x => x.Date > DateTime.UtcNow.Date)
                .Unless(x => !x.IsValid)
                .WithMessage(DeliveryDateInThePastErrorMessage)
                .Must(x => x.Date >= x.CommencementDate)
                .Unless(x => !x.IsValid)
                .WithMessage(x => string.Format(DeliveryDateBeforeCommencementDateErrorMessage, $"{x.CommencementDate:d MMMM yyyy}"))
                .Must(x => x.Date <= x.ContractEndDate)
                .Unless(x => !x.IsValid)
                .WithMessage(x => string.Format(DeliveryDateExceedsMaximumTermErrorMessage, $"{x.ContractEndDate:d MMMM yyyy}"))
                .OverridePropertyName(x => x.Day);
        }
    }
}
