using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    public class SelectDateModelValidator : AbstractValidator<SelectDateModel>
    {
        public const string DeliveryDateInThePastErrorMessage = "Planned delivery date must be in the future";
        public const string DeliveryDateBeforeCommencementDateErrorMessage = "Planned delivery date must be on or after your commencement date ({0})";

        public SelectDateModelValidator()
        {
            Include(new DateInputModelValidator());

            RuleFor(x => x)
                .Must(x => x.Date > DateTime.UtcNow.Date)
                .Unless(x => !x.IsValid)
                .WithMessage(DeliveryDateInThePastErrorMessage)
                .Must(x => x.Date >= x.CommencementDate)
                .Unless(x => !x.IsValid)
                .WithMessage(x => string.Format(DeliveryDateBeforeCommencementDateErrorMessage, $"{x.CommencementDate:d MMMM yyyy}"))
                .OverridePropertyName(x => x.Day);
        }
    }
}
