using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class PrivateCloud
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        [StringLength(1000)]
        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }
    }
}
