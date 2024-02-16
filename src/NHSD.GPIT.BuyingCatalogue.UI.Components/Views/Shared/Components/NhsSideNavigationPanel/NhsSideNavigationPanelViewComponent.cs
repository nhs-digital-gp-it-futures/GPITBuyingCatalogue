using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationPanel
{
    public sealed class NhsSideNavigationPanelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IList<NhsSideNavigationSectionModel> sections)
        {
            return View("NhsSideNavigationPanel", new NhsSideNavigationPanelModel
            {
                Sections = sections,
            });
        }
    }
}
