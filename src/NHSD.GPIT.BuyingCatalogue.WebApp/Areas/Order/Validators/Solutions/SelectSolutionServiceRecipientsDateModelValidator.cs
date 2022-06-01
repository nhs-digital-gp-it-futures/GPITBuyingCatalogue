using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipientsDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public class SelectSolutionServiceRecipientsDateModelValidator : AbstractValidator<SelectSolutionServiceRecipientsDateModel>
    {
        public SelectSolutionServiceRecipientsDateModelValidator()
        {
            RuleFor(m => m.DeliveryDate)
                .NotNull()
                .WithMessage("Planned delivery date must be a real date")
                .Must(d => d > DateTime.UtcNow)
                .WithMessage("Planned delivery date must be in the future")
                .Must(NotExceedCommencementDate)
                .WithMessage($"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement")
                .OverridePropertyName(m => m.Day);
        }

        private static bool NotExceedCommencementDate(SelectSolutionServiceRecipientsDateModel model, DateTime? deliveryDate)
            => !(model.CommencementDate.HasValue && deliveryDate > model.CommencementDate.Value.AddMonths(ValidationConstants.MaxDeliveryMonthsFromCommencement));
    }
}
