using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSecondaryButton
{
    public sealed class NhsSecondaryButtonViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text, string href)
        {
            var model = new NhsSecondaryButtonModel
            {
                Text = text,
                Href = href,
            };

            return await Task.FromResult(View("NhsSecondaryButton", model));
        }
    }
}
