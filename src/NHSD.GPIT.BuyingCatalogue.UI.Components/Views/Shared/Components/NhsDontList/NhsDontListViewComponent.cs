using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsDontList
{
    public sealed class NhsDontListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string> items)
        {
            return await Task.FromResult(View("NhsDontList", items));
        }
    }
}
