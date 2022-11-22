using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsDoList
{
    public sealed class NhsDoListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, List<string> items)
        {
            return await Task.FromResult(View("NhsDoList", new NhsDoListModel
            {
                Title = title,
                Items = items,
            }));
        }
    }
}
