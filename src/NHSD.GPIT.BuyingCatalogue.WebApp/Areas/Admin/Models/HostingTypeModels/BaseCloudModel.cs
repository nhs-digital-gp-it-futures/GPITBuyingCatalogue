using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public class BaseCloudModel : NavBaseModel
    {
        public BaseCloudModel()
        {
            BackLink = "./";
            BackLinkText = "Go back";
        }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public string RequiresHscn { get; set; }

        [Required(ErrorMessage = "Enter a summary")]
        [StringLength(500)]
        public string Summary { get; set; }

        public bool RequiresHscnChecked
        {
            get => !string.IsNullOrWhiteSpace(RequiresHscn);
            set => RequiresHscn = value ? "End user devices must be connected to HSCN/N3" : null;
        }

        public bool IsComplete => IsValid();

        public virtual bool IsValid() =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(Link) ||
            !string.IsNullOrWhiteSpace(RequiresHscn);
    }
}
