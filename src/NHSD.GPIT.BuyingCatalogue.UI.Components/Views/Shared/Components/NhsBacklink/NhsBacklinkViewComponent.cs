﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsBacklink
{
    public sealed class NhsBacklinkViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string url, string text)
        {
            var model = new NhsBacklinkModel
            {
                Href = url,
                Text = text,
            };

            return await Task.FromResult(View("Backlink", model));
        }
    }
}
