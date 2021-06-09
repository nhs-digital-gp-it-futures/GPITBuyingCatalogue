using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsWarningCallout
{
    public sealed class NhsWarningCalloutViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, string text)
        {
            var model = new NhsWarningCalloutModel
            {
                Title = title,
                Text = text,
            };

            return await Task.FromResult(View("NhsWarningCallout", model));
        }
    }
}
