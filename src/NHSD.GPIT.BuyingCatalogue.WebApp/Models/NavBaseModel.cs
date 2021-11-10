using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class NavBaseModel
    {
        public const string BackLinkDefault = "./";

        public const string BackLinkTextDefault = "Go back";

        public string BackLink { get; set; } = BackLinkDefault;

        public string BackLinkText { get; set; } = BackLinkTextDefault;
    }
}
