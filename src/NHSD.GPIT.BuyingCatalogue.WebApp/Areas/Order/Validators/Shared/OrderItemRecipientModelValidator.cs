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
            RuleFor(r => r.Quantity)
                .IsValidQuantity();
        }
    }
}
