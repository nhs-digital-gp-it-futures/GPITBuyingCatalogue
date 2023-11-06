using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

public class SelectRecipientsModelValidator : AbstractValidator<SelectRecipientsModel>
{
    public const string NoSelectionMadeErrorMessage = "Select a Service Recipient";
    public const string SelectAtLeastErrorMessage = "Select at least {0} Service Recipients";

    public SelectRecipientsModelValidator()
    {
        RuleFor(x => x.SubLocations)
            .Must(HaveMadeASelection)
            .WithMessage(NoSelectionMadeErrorMessage)
            .OverridePropertyName("SubLocations[0].ServiceRecipients[0].Selected")
            .When(m => !m.SelectAtLeast.HasValue);

        RuleFor(x => x.SubLocations)
            .Must((m, x) => HaveMadeMinimumCountSelection(m.SelectAtLeast.Value, x))
            .WithMessage(m => string.Format(SelectAtLeastErrorMessage, m.SelectAtLeast.Value))
            .OverridePropertyName("SubLocations[0].ServiceRecipients[0].Selected")
            .When(m => m.SelectAtLeast.HasValue);
    }

    private static bool HaveMadeMinimumCountSelection(int selectAtLeast, SublocationModel[] subLocations)
    {
        if (subLocations is null || !subLocations.Any())
            return false;

        var serviceRecipients = subLocations
            .Where(x => x.ServiceRecipients != null)
            .SelectMany(x => x.ServiceRecipients)
            .Where(x => x.Selected)
            .ToList();

        return serviceRecipients.Count >= selectAtLeast;
    }

    private static bool HaveMadeASelection(SublocationModel[] subLocations)
    {
        if (subLocations is null || !subLocations.Any())
            return false;

        var serviceRecipients = subLocations
            .Where(x => x.ServiceRecipients != null)
            .SelectMany(x => x.ServiceRecipients).ToList();

        return !serviceRecipients.Any()
            || serviceRecipients.Any(x => x.Selected);
    }
}
