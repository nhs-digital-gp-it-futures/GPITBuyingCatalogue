using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsDeleteButton
{
    public sealed class NhsDeleteButtonViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text, string href)
        {
            var model = new NhsDeleteButtonModel
            {
                Text = text,
                Href = href,
            };

            return await Task.FromResult(View("NhsDeleteButton", model));
        }
    }
}
