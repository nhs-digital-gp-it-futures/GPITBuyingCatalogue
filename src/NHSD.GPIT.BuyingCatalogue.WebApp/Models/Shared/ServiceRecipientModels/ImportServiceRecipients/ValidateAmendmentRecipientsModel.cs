using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

public class ValidateAmendmentRecipientsModel : NavBaseModel
{
    [ExcludeFromCodeCoverage]
    public ValidateAmendmentRecipientsModel()
    {
    }

    public string CancelLink { get; set; }

    public string ContinueLink { get; set; }

    public bool HasMissing { get; set; }

    public IReadOnlyList<ServiceRecipientModel> NewRecipients { get; set; }
}
