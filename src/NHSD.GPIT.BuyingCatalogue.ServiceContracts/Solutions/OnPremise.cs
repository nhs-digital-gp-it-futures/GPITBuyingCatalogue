using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class OnPremise
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        [StringLength(1000)]
        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public virtual bool? IsValid() =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(Link) ||
            !string.IsNullOrWhiteSpace(RequiresHscn) ||
            !string.IsNullOrWhiteSpace(HostingModel);
    }
}
