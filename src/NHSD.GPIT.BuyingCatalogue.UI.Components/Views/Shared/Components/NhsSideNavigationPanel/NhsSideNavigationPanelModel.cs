using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationPanel
{
    public class NhsSideNavigationPanelModel
    {
        public IList<NhsSideNavigationSectionModel> Sections { get; set; }
    }
}
