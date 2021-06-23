using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsBreadcumbs
{
    public sealed class NhsBreadcrumbsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<NhsBreadcrumbModel> items)
        {
            return await Task.FromResult(View("Breadcrumbs", items));
        }
    }
}
