using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Shared
{
    public sealed class OrderItemRecipientModelValidator : AbstractValidator<OrderItemRecipientModel>
    {
        public OrderItemRecipientModelValidator(DateTime? commencementDate = null)
        {
            RuleFor(r => r.DeliveryDate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Planned delivery date must be a real date")
                .Must(d => d > DateTime.UtcNow)
                .WithMessage("Planned delivery date must be in the future")
                .Must(deliveryDate => NotExceedCommencementDate(commencementDate, deliveryDate))
                .WithMessage($"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement")
                .OverridePropertyName(p => p.Day);

            RuleFor(r => r.Quantity)
                .IsValidQuantity();
        }

        private static bool NotExceedCommencementDate(DateTime? commencementDate, DateTime? deliveryDate)
            => !(commencementDate.HasValue && deliveryDate > commencementDate.Value.AddMonths(ValidationConstants.MaxDeliveryMonthsFromCommencement));
    }
}
