﻿using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.FundingSources
{
    public sealed class ConfirmFrameworkChangeModelValidator : AbstractValidator<ConfirmFrameworkChangeModel>
    {
        public const string ConfirmChangeErrorMessage = "Select yes if you want to confirm your changes";

        public ConfirmFrameworkChangeModelValidator()
        {
            RuleFor(m => m.ConfirmChanges)
                .NotNull()
                .WithMessage(ConfirmChangeErrorMessage);
        }
    }
}
