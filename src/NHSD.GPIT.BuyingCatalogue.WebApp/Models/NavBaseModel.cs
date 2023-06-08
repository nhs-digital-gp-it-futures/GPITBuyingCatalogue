using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class NavBaseModel : IPageTitleModel
    {
        public const string BackLinkDefault = "./";

        public const string BackLinkTextDefault = "Go back";

        public string BackLink { get; set; } = BackLinkDefault;

        public string BackLinkText { get; set; } = BackLinkTextDefault;

        public virtual string Title { get; set; }

        public virtual string Caption { get; set; }

        public virtual string Advice { get; set; }

        public virtual string AdditionalAdvice { get; set; }
    }
}
