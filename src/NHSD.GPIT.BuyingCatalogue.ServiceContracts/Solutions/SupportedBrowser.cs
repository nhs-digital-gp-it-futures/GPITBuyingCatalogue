using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public sealed class SupportedBrowser
    {
        public string BrowserName { get; set; }

        [MaxLength(50)]
        public string MinimumBrowserVersion { get; set; }
    }
}
