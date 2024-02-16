using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection
{
    public sealed class NhsSideNavigationSectionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string name, string controller, string action, Dictionary<string, string> routeData, bool selected = false, bool show = true)
        {
            return View("NhsSideNavigationSection", new NhsSideNavigationSectionModel
            {
                Name = name,
                Controller = controller,
                Action = action,
                RouteData = routeData,
                Selected = selected,
                Show = show,
            });
        }
    }
}
