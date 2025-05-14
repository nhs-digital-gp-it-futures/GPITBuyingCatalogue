using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsBacklink
{
    public sealed class NhsBacklinkViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string url, string text)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"{nameof(url)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"{nameof(text)} cannot be null or empty");
            }

            if (!Url.IsLocalUrl(url) && url != "./")
            {
                throw new InvalidOperationException($"Url validation failed: {url}");
            }

            var model = new NhsBacklinkModel
            {
                Href = url,
                Text = text,
            };

            return await Task.FromResult(View("Backlink", model));
        }
    }
}
