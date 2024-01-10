using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public class UploadOrSelectServiceRecipientModel : NavBaseModel
{
    public const string YesUploadRecipientsOption = "Upload Service Recipients using a CSV file";
    public const string NoUploadRecipientsOption = "Select Service Recipients manually";

    public bool? ShouldUploadRecipients { get; set; }

    public override string Title => "Service Recipients";

    public override string Advice => "Select how you want to add Service Recipients.";

    public IEnumerable<SelectOption<bool>> ServiceRecipientOptions => new List<SelectOption<bool>>
        {
            new(YesUploadRecipientsOption, true),
            new(NoUploadRecipientsOption, false),
        };
}
