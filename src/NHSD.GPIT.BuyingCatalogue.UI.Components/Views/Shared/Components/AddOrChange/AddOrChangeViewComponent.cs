using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.AddOrChange;

public sealed class AddOrChangeViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string href, bool isChange, string text)
    {
        var model = new AddOrChangeModel { Href = href, IsChange = isChange, Text = text };

        return await Task.FromResult(View("AddOrChange", model));
    }
}
