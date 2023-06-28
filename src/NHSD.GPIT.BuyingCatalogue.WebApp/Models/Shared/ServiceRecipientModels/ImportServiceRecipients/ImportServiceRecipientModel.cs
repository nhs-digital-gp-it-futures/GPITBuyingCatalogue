using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

public class ImportServiceRecipientModel : NavBaseModel
{
    public string DownloadTemplateLink { get; set; }

    public IFormFile File { get; set; }
}
