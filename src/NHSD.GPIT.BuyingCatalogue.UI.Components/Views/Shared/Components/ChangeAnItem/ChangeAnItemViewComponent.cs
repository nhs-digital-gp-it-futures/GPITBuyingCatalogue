using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.ChangeAnItem;

public sealed class ChangeAnItemViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string href, string text)
    {
        var model = new ChangeAnItemModel { Href = href, Text = text };

        return await Task.FromResult(View("ChangeAnItem", model));
    }
}
