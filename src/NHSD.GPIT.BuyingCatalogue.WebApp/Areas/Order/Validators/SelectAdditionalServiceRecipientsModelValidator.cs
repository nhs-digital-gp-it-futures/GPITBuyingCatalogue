using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public sealed class SelectAdditionalServiceRecipientsModelValidator : AbstractValidator<SelectAdditionalServiceRecipientsModel>
    {
        public SelectAdditionalServiceRecipientsModelValidator()
        {
            RuleFor(m => m.ServiceRecipients)
                .Must(m => m.Any(sr => sr.Selected))
                .WithMessage("Select a Service Recipient")
                .OverridePropertyName("ServiceRecipients[0].Selected");
        }
    }
}
