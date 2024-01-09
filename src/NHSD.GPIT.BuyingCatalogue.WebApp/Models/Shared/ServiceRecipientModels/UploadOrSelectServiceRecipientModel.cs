using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public class UploadOrSelectServiceRecipientModel : NavBaseModel
{
    public const string YesUploadRecipientsOption = "Upload Service Recipients using a CSV file";
    public const string NoUploadRecipientsOption = "Select Service Recipients manually";

    public string SelectedServiceRecipient { get; set; }

    public bool? ShouldUploadRecipients { get; set; }

    public string ImportRecipientsLink { get; set; }

    public override string Title
    {
        get => base.Title ?? "Service Recipients";
        set => base.Title = value;
    }

    public override string Advice
    {
        get => base.Advice ?? "Select how you want to add Service Recipients.";
        set => base.Advice = value;
    }

    public IEnumerable<SelectOption<bool>> ServiceRecipientOptions => new List<SelectOption<bool>>
        {
            new(YesUploadRecipientsOption, true),
            new(NoUploadRecipientsOption, false),
        };
}
