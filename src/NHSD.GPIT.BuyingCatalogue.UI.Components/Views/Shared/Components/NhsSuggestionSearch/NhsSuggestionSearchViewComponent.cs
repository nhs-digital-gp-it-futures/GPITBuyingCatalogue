using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSuggestionSearch
{
    public sealed class NhsSuggestionSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string id, string ajaxUrl, string queryParameterName, string titleText, bool? hideLabel = null, string placeholderText = null)
        {
            var model = new NhsSuggestionSearchModel
            {
                Id = id,
                AjaxUrl = ajaxUrl,
                QueryParameterName = queryParameterName,
                TitleText = titleText,
                CurrentPageUrl = UriHelper.GetEncodedPathAndQuery(HttpContext.Request),
                SearchText = HttpContext.Request.Query[queryParameterName],
                HideLabel = hideLabel,
                PlaceholderText = placeholderText,
            };

            return await Task.FromResult(View("NhsSuggestionSearch", model));
        }
    }
}
