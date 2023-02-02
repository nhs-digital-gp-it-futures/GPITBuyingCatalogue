using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class NavBaseModel : PageTitleModel
    {
        public const string BackLinkDefault = "./";

        public const string BackLinkTextDefault = "Go back";

        public string BackLink { get; set; } = BackLinkDefault;

        public string BackLinkText { get; set; } = BackLinkTextDefault;
    }
}
