using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.InlineInformation
{
    public sealed class InlineInformationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text)
        {
            var model = new InlineInformationModel { Text = text };

            return await Task.FromResult(View("InlineInformation", model));
        }
    }
}
