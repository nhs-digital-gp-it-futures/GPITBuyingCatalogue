using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.DeliveryDates
{
    public class RecipientDateModelValidator : AbstractValidator<RecipientDateModel>
    {
        public const string DeliveryDateInThePastErrorMessage = "Date for {0} must be in the future";
        public const string DeliveryDateBeforeCommencementDateErrorMessage = "Date for {0} must be on or after your commencement date ({1})";

        public RecipientDateModelValidator()
        {
            Include(new DateInputModelValidator());

            RuleFor(x => x)
                .Must(x => x.Date > DateTime.UtcNow.Date)
                .Unless(x => !x.IsValid)
                .WithMessage(x => string.Format(DeliveryDateInThePastErrorMessage, x.Description))
                .Must(x => x.Date >= x.CommencementDate)
                .Unless(x => !x.IsValid)
                .WithMessage(x => CommencementDateErrorMessage(x.Description, x.CommencementDate))
                .OverridePropertyName(x => x.Day);
        }

        public static string CommencementDateErrorMessage(string description, DateTime commencementDate)
        {
            return string.Format(
                DeliveryDateBeforeCommencementDateErrorMessage,
                description,
                $"{commencementDate:d MMMM yyyy}");
        }
    }
}
