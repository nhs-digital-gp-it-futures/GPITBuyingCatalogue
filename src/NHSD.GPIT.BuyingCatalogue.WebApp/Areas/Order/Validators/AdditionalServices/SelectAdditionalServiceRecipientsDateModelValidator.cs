using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices
{
    public class SelectAdditionalServiceRecipientsDateModelValidator : AbstractValidator<SelectAdditionalServiceRecipientsDateModel>
    {
        public SelectAdditionalServiceRecipientsDateModelValidator()
        {
            RuleFor(m => m.DeliveryDate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Planned delivery date must be a real date")
                .Must(d => d > DateTime.UtcNow)
                .WithMessage("Planned delivery date must be in the future")
                .Must(NotExceedCommencementDate)
                .WithMessage($"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement")
                .OverridePropertyName(m => m.Day);
        }

        private static bool NotExceedCommencementDate(SelectAdditionalServiceRecipientsDateModel model, DateTime? deliveryDate)
            => !(model.CommencementDate.HasValue && deliveryDate > model.CommencementDate.Value.AddMonths(ValidationConstants.MaxDeliveryMonthsFromCommencement));
    }
}
