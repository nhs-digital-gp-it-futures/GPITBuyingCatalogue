using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

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

    private static bool HaveMadeASelection(SublocationModel[] subLocations)
    {
        if (subLocations is null || !subLocations.Any())
            return false;

        var serviceRecipients = subLocations.SelectMany(x => x.ServiceRecipients).ToList();

        return !serviceRecipients.Any()
            || serviceRecipients.Any(x => x.Selected);
    }
}
