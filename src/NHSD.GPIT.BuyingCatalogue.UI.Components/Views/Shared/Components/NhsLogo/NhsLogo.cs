using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsLogo
{
    public sealed class NhsLogo : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult(View("NhsLogo"));
        }
    }
}
