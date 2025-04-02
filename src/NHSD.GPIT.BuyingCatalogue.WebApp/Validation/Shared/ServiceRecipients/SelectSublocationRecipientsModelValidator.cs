using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients
{
    public class SelectSublocationRecipientsModelValidator : AbstractValidator<SelectSublocationRecipientsModel>
    {
        public const string NoRecipientsSelectedMessage = "Select the service recipients for this order";

        public SelectSublocationRecipientsModelValidator()
        {
            RuleFor(x => x.RenderedServiceRecipients)
                .Must(HaveMadeASelection)
                .WithMessage(NoRecipientsSelectedMessage)
                .OverridePropertyName("RenderedServiceRecipients[0].Selected");
        }

        private static bool HaveMadeASelection(IReadOnlyList<ServiceRecipientModel> checkboxSelections)
        {
            if (checkboxSelections is null || checkboxSelections.Count == 0)
                return false;

            var checkedCheckboxSelectionCount =
                checkboxSelections.Count(x => x.Selected);

            return checkedCheckboxSelectionCount > 0;
        }
    }
}
