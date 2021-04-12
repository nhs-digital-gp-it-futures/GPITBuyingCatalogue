using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
