using System.Web;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch
{
    public sealed class HtmlEncodedSuggestionSearchResult(string title, string category, string url)
    {
        public string Title { get; } = HttpUtility.HtmlEncode(title);

        public string Category { get; } = HttpUtility.HtmlEncode(category);

        public string Url { get; } = url;
    }
}
