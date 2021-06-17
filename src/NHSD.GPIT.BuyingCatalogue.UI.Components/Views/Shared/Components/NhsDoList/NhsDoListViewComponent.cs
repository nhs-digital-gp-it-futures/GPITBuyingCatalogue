using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsDoDontList
{
    public sealed class NhsDoListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string> items)
        {
            return await Task.FromResult(View("NhsDoList", items));
        }
    }
}
