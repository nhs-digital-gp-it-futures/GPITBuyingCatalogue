using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.ActionLink
{
    public sealed class NhsActionLinkViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string url, string text)
        {
            var model = new ActionLinkModel
            {
                Url = url,
                Text = text,
            };

            return await Task.FromResult(View("ActionLink", model));
        }
    }
}
