using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Helpers
{
    /// <summary>
    /// Contains source-generated Regex validators.
    /// </summary>
    public static partial class TagHelperRegex
    {
        [GeneratedRegex("&lt;(.*?)&gt;", RegexOptions.IgnoreCase)]
        public static partial Regex HiddenContentRegex();
    }
}
