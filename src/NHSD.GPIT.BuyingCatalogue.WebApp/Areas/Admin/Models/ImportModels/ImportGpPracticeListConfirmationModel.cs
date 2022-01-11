using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels
{
    public sealed class ImportGpPracticeListConfirmationModel : NavBaseModel
    {
        public ImportGpPracticeListConfirmationModel(string csvUrl, string emailAddress)
        {
            CsvUrl = csvUrl;
            EmailAddress = emailAddress;
        }

        public string CsvUrl { get; set; }

        public string EmailAddress { get; set; }
    }
}
