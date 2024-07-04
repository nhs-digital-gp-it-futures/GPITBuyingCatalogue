using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsNavigationPanel
{
    public sealed class NhsNavigationPanelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string title, List<KeyValuePair<string, string>> items)
        {
            return View("NhsNavigationPanel", new NhsNavigationPanelModel
            {
                Title = title,
                Items = items,
            });
        }
    }
}
