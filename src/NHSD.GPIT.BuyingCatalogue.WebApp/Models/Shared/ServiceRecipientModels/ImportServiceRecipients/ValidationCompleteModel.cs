using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

public sealed class ValidationCompleteModel : NavBaseModel
{
    private const string ValidationSucceededAdvice =
        "We have received your CSV file. We have been able to match all ODS codes and service recipient names to what we have have on record.";

    private const string ValidationPartiallySucceededAdvice =
        "We have received your CSV file and shown you the mismatches we detected. We have been able to match all remaining ODS codes and service recipient names to what we have have on record.";

    private const string ValidationSucceededNextStep = "You will be able to confirm these changes in the next step.";

    private const string ValidationFailureAdvice =
        "We Have received your CSV file. We have not been able to match any ODS Codes to what we have on record.";

    private const string ValidationFailureNextStep = "Please try again, or select your service recipients manually.";

    [ExcludeFromCodeCoverage]
    public ValidationCompleteModel()
    {
    }

    public ValidationCompleteModel(
        string competitionName,
        ValidationStatus validationStatus,
        IReadOnlyList<SublocationModel> sublocations)
    {
        Title = "Upload validated";
        Caption = competitionName;
        InteractionNoun = "competition";
        Sublocations = sublocations;
        ValidationStatus = validationStatus;

        switch (validationStatus)
        {
            case ImportServiceRecipients.ValidationStatus.Success:
                AdviceBody = ValidationSucceededAdvice;
                NextStep = ValidationSucceededNextStep;
                break;

            case ImportServiceRecipients.ValidationStatus.PartialSuccess:
                AdviceBody = ValidationPartiallySucceededAdvice;
                NextStep = ValidationSucceededNextStep;
                break;

            case ImportServiceRecipients.ValidationStatus.Failure:
            default:
                Title = "Upload failed";
                AdviceBody = ValidationFailureAdvice;
                NextStep = ValidationFailureNextStep;
                break;
        }
    }

    public string AdviceBody { get; init; }

    public string NextStep { get; init; }

    public ValidationStatus? ValidationStatus { get; init; }

    public IReadOnlyList<SublocationModel> Sublocations { get; init; }

    public string InteractionNoun { get; init; }

    public string CancelHref { get; init; }
}
