using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class PublicCloud
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public string RequiresHscn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }
    }
}
