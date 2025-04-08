using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

public sealed class ValidationCompleteModel : NavBaseModel
{
    private const string ValidationSucceededAdvice =
        "We have received your CSV file. We have been able to match all ODS codes and service recipient names to what we have have on record.";

    private const string ValidationPartiallySucceededAdvice =
        "We have received your CSV file and shown you the mismatches we detected. We have been able to match all remaining ODS codes and service recipient names to what we have have on record.";

    [ExcludeFromCodeCoverage]
    public ValidationCompleteModel()
    {
    }

    public ValidationCompleteModel(
        string competitionName,
        bool hasHadValidationFailure,
        IReadOnlyList<SublocationModel> sublocations)
    {
        Title = "Upload validated";
        Caption = competitionName;
        AdviceBody = hasHadValidationFailure ? ValidationPartiallySucceededAdvice : ValidationSucceededAdvice;
        InteractionNoun = "competition";
        Sublocations = sublocations;
    }

    public string AdviceBody { get; init; }

    public IReadOnlyList<SublocationModel> Sublocations { get; init; }

    public string InteractionNoun { get; init; }

    public string CancelHref { get; init; }
}
