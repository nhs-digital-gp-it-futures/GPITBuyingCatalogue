using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsDontList
{
    public sealed class NhsDontListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, List<string> items)
        {
            return await Task.FromResult(View("NhsDontList", new NhsDontListModel
            {
                Title = title,
                Items = items,
            }));
        }
    }
}
