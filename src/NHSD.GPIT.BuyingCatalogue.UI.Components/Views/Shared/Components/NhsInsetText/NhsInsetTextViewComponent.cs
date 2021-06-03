using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.InsetText
{
    public sealed class NhsInsetTextViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text)
        {
            return await Task.FromResult(View("NhsInsetText", text));
        }
    }
}
