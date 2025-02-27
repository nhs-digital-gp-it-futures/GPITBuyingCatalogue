using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.AddOrChange;

public sealed class AddOrChangeViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(
        string href,
        string nounPhrase,
        bool isChange = false,
        string customVerb = "")
    {
        var model = new AddOrChangeModel
        {
            Href = href, NounPhrase = nounPhrase, IsChange = isChange, CustomVerb = customVerb,
        };

        return await Task.FromResult(View("AddOrChange", model));
    }
}
