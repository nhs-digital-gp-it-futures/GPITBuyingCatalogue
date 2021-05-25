using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class HybridHostingType
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
            !string.IsNullOrEmpty(Summary) ||
            !string.IsNullOrEmpty(Link) ||
            !string.IsNullOrEmpty(RequiresHscn) ||
            !string.IsNullOrEmpty(HostingModel);
    }
}
