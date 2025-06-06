using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

public class SelectMergerOrSplitRecipientsModelValidator : AbstractValidator<SelectMergerOrSplitRecipientsModel>
{
    public readonly string SelectAtLeastErrorMessage =
        $"Select at least {SelectMergerOrSplitRecipientsModel.SelectAtLeast} Service Recipients";

    public SelectMergerOrSplitRecipientsModelValidator()
    {
        RuleFor(x => x.SubLocations)
            .Must((_, x) => HaveMadeMinimumCountSelection(SelectMergerOrSplitRecipientsModel.SelectAtLeast, x))
            .WithMessage(_ => string.Format(
                SelectAtLeastErrorMessage,
                SelectMergerOrSplitRecipientsModel.SelectAtLeast))
            .OverridePropertyName("SubLocations[0].ServiceRecipients[0].Selected");
    }

    private static bool HaveMadeMinimumCountSelection(int selectAtLeast, SublocationModel[] subLocations)
    {
        if (subLocations is not { Length: > 0 })
            return false;

        return subLocations
            .Where(x => x.ServiceRecipients != null)
            .SelectMany(x => x.ServiceRecipients)
            .Count(x => x.Selected) >= selectAtLeast;
    }
}
