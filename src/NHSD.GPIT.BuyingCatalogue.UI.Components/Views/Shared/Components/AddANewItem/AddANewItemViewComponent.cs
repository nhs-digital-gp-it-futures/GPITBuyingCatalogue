using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.AddANewItem
{
    public sealed class AddANewItemViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string href, string text)
        {
            var model = new AddANewItemModel { Href = href, Text = text };

            return await Task.FromResult(View("AddANewItem", model));
        }
    }
}
