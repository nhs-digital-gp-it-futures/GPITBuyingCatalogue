using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients
{
    public class SelectRecipientsModelValidator : AbstractValidator<SelectRecipientsModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a Service Recipient";

        public SelectRecipientsModelValidator()
        {
            RuleFor(x => x.SubLocations)
                .Must(HaveMadeASelection)
                .WithMessage(NoSelectionMadeErrorMessage)
                .OverridePropertyName("SubLocations[0].ServiceRecipients[0].Selected");
        }

        private static bool HaveMadeASelection(List<SublocationModel> subLocations)
        {
            var serviceRecipients = subLocations.SelectMany(x => x.ServiceRecipients).ToList();

            return !(serviceRecipients?.Any() ?? false)
                || serviceRecipients.Any(x => x.Selected);
        }
    }
}
