using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

public sealed class ValidationCompleteModel : NavBaseModel
{
    private const string ValidationSucceededAdvice =
        "We have received your CSV file and have been able to match all ODS Codes and Service Recipient names to what we have on record.";

    private const string ValidationPartiallySucceededAdvice =
        "We have recieved your CSV file and we have been able to match all remaining ODS Codes and Service Recipient names to what we have on record. Any Service Recipients that have failed to import will not be included.";

    [ExcludeFromCodeCoverage]
    public ValidationCompleteModel()
    {
    }

    public ValidationCompleteModel(Competition competition, bool hasHadValidationFailure)
    {
        Title = "Upload validated";
        Caption = competition.Name;
        Advice = hasHadValidationFailure ? ValidationPartiallySucceededAdvice : ValidationSucceededAdvice;
        InteractionNoun = "competition";
    }

    public IReadOnlyList<SublocationModel> Sublocations { get; init; }

    public string InteractionNoun { get; init; }

    public string CancelHref { get; init; }
}
