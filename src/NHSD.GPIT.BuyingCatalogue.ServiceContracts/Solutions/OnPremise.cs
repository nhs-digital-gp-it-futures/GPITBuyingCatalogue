using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class OnPremise
    {
        public string Summary { get; set; }

        public string Link { get; set; }

        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        public virtual bool? IsValid() =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(Link) ||
            !string.IsNullOrWhiteSpace(RequiresHscn) ||
            !string.IsNullOrWhiteSpace(HostingModel);
    }
}
