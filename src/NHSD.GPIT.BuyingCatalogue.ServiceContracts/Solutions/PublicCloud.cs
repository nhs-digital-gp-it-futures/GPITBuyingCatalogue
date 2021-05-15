using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class PublicCloud
    {
        [StringLength(500)]
        public string Summary { get; set; }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public string RequiresHscn { get; set; }

        public bool? IsValid() =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(Link) ||
            !string.IsNullOrWhiteSpace(RequiresHscn);
    }
}
