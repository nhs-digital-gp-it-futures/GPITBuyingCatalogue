using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSuggestionSearch
{
    public sealed class NhsSuggestionSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string id, string ajaxUrl, string queryParameterName, string titleText, string placeholderText = "")
        {
            var model = new NhsSuggestionSearchModel
            {
                Id = id,
                AjaxUrl = ajaxUrl,
                QueryParameterName = queryParameterName,
                TitleText = titleText,
                PlaceholderText = placeholderText,
                CurrentPageUrl = UriHelper.GetEncodedPathAndQuery(HttpContext.Request),
                SearchText = HttpContext.Request.Query[queryParameterName],
            };

            return await Task.FromResult(View("NhsSuggestionSearch", model));
        }
    }
}
