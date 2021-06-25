using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSecondaryButton
{
    public sealed class NhsSecondaryButtonViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text, string url, bool usePrimaryColour)
        {
            var model = new NhsSecondaryButtonModel
            {
                Text = text,
                Url = url,
                UsePrimaryColour = usePrimaryColour,
            };

            return await Task.FromResult(View("NhsSecondaryButton", model));
        }
    }
}
