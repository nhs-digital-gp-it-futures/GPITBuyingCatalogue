using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels
{
    public sealed class ImportGpPracticeListModel : NavBaseModel
    {
        [StringLength(100)]
        public string CsvUrl { get; set; }
    }
}
