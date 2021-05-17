using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class PrivateCloud
    {
        [StringLength(500)]
        public string Summary { get; set; }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        [StringLength(1000)]
        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        public virtual bool? IsValid() =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(Link) ||
            !string.IsNullOrWhiteSpace(RequiresHscn) ||
            !string.IsNullOrWhiteSpace(HostingModel);
    }
}
