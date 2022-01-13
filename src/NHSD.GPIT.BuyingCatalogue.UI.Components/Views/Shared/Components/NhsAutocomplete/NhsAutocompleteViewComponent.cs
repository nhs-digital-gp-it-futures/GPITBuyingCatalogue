using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Autocomplete
{
    public sealed class NhsAutocompleteViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string id, string ajaxUrl, string queryParameterName, string placeholderText)
        {
            var model = new NhsAutocompleteModel
            {
                Id = id,
                AjaxUrl = ajaxUrl,
                QueryParameterName = queryParameterName,
                PlaceholderText = placeholderText,
            };

            return await Task.FromResult(View("NhsAutocomplete", model));
        }
    }
}
