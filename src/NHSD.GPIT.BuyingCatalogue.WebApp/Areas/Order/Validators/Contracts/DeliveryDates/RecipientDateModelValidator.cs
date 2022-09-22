using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.DeliveryDates
{
    public class RecipientDateModelValidator : AbstractValidator<RecipientDateModel>
    {
        public const string DeliveryDateInThePastErrorMessage = "Planned delivery date must be in the future";

        public RecipientDateModelValidator()
        {
            Include(new DateInputModelValidator());

            RuleFor(x => x)
                .Must(x => x.Date > DateTime.UtcNow.Date)
                .Unless(x => !x.IsValid)
                .WithMessage(DeliveryDateInThePastErrorMessage)
                .OverridePropertyName(x => x.Day);
        }
    }
}
