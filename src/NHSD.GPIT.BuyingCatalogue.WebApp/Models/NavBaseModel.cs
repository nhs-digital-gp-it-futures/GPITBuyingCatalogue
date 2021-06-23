using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class NavBaseModel
    {
        public string BackLink { get; set; } = "./";

        public string BackLinkText { get; set; } = "Go back";
    }
}
