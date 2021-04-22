using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class NavBaseModel
    {
        public IDictionary<string, string> BackLinkAttributes { get; set; }

        public string BackLinkClasses { get; set; }

        public string BackLink { get; set; } = "./";

        public string BackLinkHtml { get; set; }

        public string BackLinkText { get; set; } = "Go back to previous page";

        public string BackLinkAttributesString()
        {
            if (BackLinkAttributes == null || !BackLinkAttributes.Any())
                return string.Empty;

            return string.Join(" ", BackLinkAttributes.Select(x => $"{x.Key}={x.Value}"));
        }

        public bool BackLinkHasHtml() => !string.IsNullOrWhiteSpace(BackLinkHtml);
    }
}