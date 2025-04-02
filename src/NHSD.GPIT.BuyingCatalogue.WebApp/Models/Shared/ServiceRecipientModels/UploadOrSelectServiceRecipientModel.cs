using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public class UploadOrSelectServiceRecipientModel : NavBaseModel
{
    private const string CsvRecipientsOption = "Upload a CSV file";
    private const string CsvRecipientsHintText = "Create a list of service recipients and upload using a CSV file";

    private const string ManualRecipientsOption = "Add service recipients manually";
    private const string ManualRecipientsHintText = "Select the service recipients from a list of organisations";

    public bool? ShouldUploadRecipients { get; set; }

    public override string Title => "Service recipients";

    public override string Advice => "Select how you want to add service recipients.";

    public IEnumerable<SelectOption<bool>> ServiceRecipientOptions => new List<SelectOption<bool>>
        {
            new(CsvRecipientsOption, CsvRecipientsHintText, true),
            new(ManualRecipientsOption, ManualRecipientsHintText, false),
        };
}
