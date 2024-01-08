using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public class UploadOrSelectServiceRecipientModel : NavBaseModel
{
    public string SelectedServiceRecipientOptions { get; set; }

    public string ImportRecipientsLink { get; set; }

    public IEnumerable<SelectOption<string>> ServiceRecipientOptions => new List<SelectOption<string>>
        {
            new("Upload Service Recipients using a CSV file", "Upload"),
            new("Select Service Recipients manually", "Select"),
        };
}
