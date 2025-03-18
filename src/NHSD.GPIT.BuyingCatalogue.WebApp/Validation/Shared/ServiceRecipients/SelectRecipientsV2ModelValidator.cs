using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients
{
    public class SelectRecipientsV2ModelValidator : AbstractValidator<SelectRecipientsV2Model>
    {
        private const string NoRecipientsSelectedMessage = "Select the service recipients for this order";

        public SelectRecipientsV2ModelValidator()
        {
            RuleFor(x => x.RenderedServiceRecipients)
                .Must(HaveMadeASelection)
                .WithMessage(NoRecipientsSelectedMessage)
                .OverridePropertyName("RenderedServiceRecipients[0].Value");
        }

        private static bool HaveMadeASelection(List<ServiceRecipientModel> checkboxSelections)
        {
            if (checkboxSelections is null || checkboxSelections.Count == 0)
                return false;

            var checkedCheckboxSelectionCount =
                checkboxSelections.Count(x => x.Selected);

            return checkedCheckboxSelectionCount > 0;
        }
    }
}
