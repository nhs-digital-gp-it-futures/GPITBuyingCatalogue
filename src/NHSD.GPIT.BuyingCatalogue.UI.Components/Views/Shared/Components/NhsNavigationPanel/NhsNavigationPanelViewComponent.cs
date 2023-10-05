using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsNavigationPanel
{
    public sealed class NhsNavigationPanelViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, List<KeyValuePair<string, string>> items)
        {
            return await Task.FromResult(View("NhsNavigationPanel", new NhsNavigationPanelModel
            {
                Title = title,
                Items = items,
            }));
        }
    }
}
