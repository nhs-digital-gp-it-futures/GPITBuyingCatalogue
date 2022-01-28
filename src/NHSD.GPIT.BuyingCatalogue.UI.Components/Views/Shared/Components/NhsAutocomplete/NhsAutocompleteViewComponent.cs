using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Autocomplete
{
    public sealed class NhsAutocompleteViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string id, string ajaxUrl, string queryParameterName, string titleText)
        {
            var model = new NhsAutocompleteModel
            {
                Id = id,
                AjaxUrl = ajaxUrl,
                QueryParameterName = queryParameterName,
                TitleText = titleText,
                CurrentPageUrl = UriHelper.GetEncodedPathAndQuery(HttpContext.Request),
            };

            return await Task.FromResult(View("NhsAutocomplete", model));
        }
    }
}
