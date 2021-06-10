using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsDeleteButton
{
    public sealed class NhsDeleteButtonViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text, string url)
        {
            var model = new NhsDeleteButtonModel
            {
                Text = text,
                Url = url,
            };

            return await Task.FromResult(View("NhsDeleteButton", model));
        }
    }
}
