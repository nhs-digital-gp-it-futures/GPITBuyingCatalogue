using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public sealed class CommencementDateModelValidator : AbstractValidator<CommencementDateModel>
    {
        public CommencementDateModelValidator()
        {
            RuleFor(m => m.CommencementDate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Commencement date must be a real date")
                .Must(d => d > DateTime.UtcNow.AddDays(-60))
                .WithMessage("Commencement date must be in the future or within the last 60 days")
                .OverridePropertyName(m => m.Day);
        }
    }
}
