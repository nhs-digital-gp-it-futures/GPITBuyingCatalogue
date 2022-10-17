using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.DeliveryDates
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
                .WithMessage(x => string.Format(DeliveryDateBeforeCommencementDateErrorMessage, $"{x.CommencementDate:dd MMMM yyyy}"))
                .OverridePropertyName(x => x.Day);
        }
    }
}
