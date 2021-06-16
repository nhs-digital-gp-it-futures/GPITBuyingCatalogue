using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsBreadcumbs
{
    public sealed class NhsBreadcrumbsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Dictionary<string, string> items)
        {
            return await Task.FromResult(View("Breadcrumbs", items));
        }
    }
}
