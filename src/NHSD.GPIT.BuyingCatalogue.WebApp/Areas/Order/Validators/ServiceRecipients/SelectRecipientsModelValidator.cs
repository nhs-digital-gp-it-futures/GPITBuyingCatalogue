using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.ServiceRecipients
{
    public class SelectRecipientsModelValidator : AbstractValidator<SelectRecipientsModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a Service Recipient";

        public SelectRecipientsModelValidator()
        {
            RuleFor(x => x.ServiceRecipients)
                .Must(HaveMadeASelection)
                .WithMessage(NoSelectionMadeErrorMessage)
                .OverridePropertyName("ServiceRecipients[0].Selected");
        }

        private static bool HaveMadeASelection(List<ServiceRecipientModel> serviceRecipients)
        {
            return serviceRecipients.Any(x => x.Selected);
        }
    }
}
