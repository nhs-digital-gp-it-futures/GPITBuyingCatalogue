using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsImages
{
    public sealed class NhsImageViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string url, string alt, string caption)
        {
            var model = new NhsImageModel
            {
                ImageUrl = url,
                ImageAltText = alt,
                ImageCaption = caption,
            };

            return await Task.FromResult(View("NhsImage", model));
        }
    }
}
